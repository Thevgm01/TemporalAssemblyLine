﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GrabController : MonoBehaviour
{
    public Action cannotGrab = delegate { };
    public Action<Transform> canGrab = delegate { };
    public Action<Transform> grabbed = delegate { };
    public Action<Transform> letGo = delegate { };

    Transform _head;
    Collider _body;

    [SerializeField]
    [Range(0f, 5f)]
    float maxGrabDistance = 3f;
    [SerializeField]
    [Range(0f, 10f)]
    float maxGrabSpeed = 2f;
    [SerializeField]
    [Range(0f, 30f)]
    float grabForce = 10f;
    [SerializeField]
    LayerMask grabLayer;

    private float holdDistance;

    private Rigidbody currentLookRB;
    private Rigidbody currentGrabRB;

    private bool _grabbing;
    public bool isGrabbing
    {
        get => _grabbing;
    }

    // Start is called before the first frame update
    void Awake()
    {
        _body = GetComponent<Collider>();
        _head = transform.Find("Head");
    }

    void Update()
    {
        if (currentGrabRB == null)
        {
            Physics.Raycast(_head.position, _head.forward, out var ray, maxGrabDistance, grabLayer);
            //if(ray.collider != null) Debug.DrawLine(_head.position, ray.point);

            if (ray.collider != null) // Looking at an object in the grabbable layer
            {
                holdDistance = Vector3.Distance(_head.position, ray.collider.transform.position);
                if (currentLookRB == null || // Wasn't looking at any object last frame, or...
                    ray.collider.gameObject != currentLookRB.gameObject) // Looking at a different object this frame)
                {
                    try
                    {
                        currentLookRB = ray.collider.GetComponent<Rigidbody>();
                        canGrab?.Invoke(currentLookRB.transform);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e, ray.collider.gameObject);
                    }
                }
            }
            else
            {
                currentLookRB = null;
                cannotGrab?.Invoke();
            }
        }
    }

    void FixedUpdate()
    {
        if (currentGrabRB != null)
        {
            Vector3 posDif = _head.position + (_head.forward * holdDistance) - currentGrabRB.position;
            currentGrabRB.AddForce(posDif * posDif.sqrMagnitude * grabForce);
            currentGrabRB.velocity *= 0.9f;

            if (currentGrabRB.velocity.magnitude > maxGrabSpeed)
                currentGrabRB.velocity = currentGrabRB.velocity.normalized * maxGrabSpeed;

            //armTarget.MoveRotation(Quaternion.Euler(0f, angleX, 0f));
            currentGrabRB.angularVelocity *= 0.9f;
        }
    }

    public void Grab()
    {
        if (currentLookRB == null) return;

        currentGrabRB = currentLookRB;
        currentGrabRB.useGravity = false;
        Physics.IgnoreCollision(_body, currentGrabRB.GetComponent<Collider>(), true);
        grabbed?.Invoke(currentGrabRB.transform);
        _grabbing = true;
    }

    public void Release()
    {
        _grabbing = false;
        letGo?.Invoke(currentGrabRB.transform);
        StartCoroutine("EnableCollisionsOnExit", currentGrabRB.GetComponent<Collider>());
        currentGrabRB.useGravity = true;
        currentGrabRB = null;
    }

    IEnumerator EnableCollisionsOnExit(Collider grabbedCollider)
    {
        while (_body.bounds.Intersects(grabbedCollider.bounds))
        {
            yield return new WaitForSeconds(0.1f);
        }
        Physics.IgnoreCollision(_body, grabbedCollider.GetComponent<Collider>(), false);
    }
}
