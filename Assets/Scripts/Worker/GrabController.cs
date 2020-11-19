using System.Collections;
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
    Collider _feet;

    [SerializeField]
    [Range(0f, 5f)]
    float maxGrabDistance = 3f;
    [SerializeField]
    [Range(0f, 10f)]
    float maxGrabSpeed = 2f;
    [SerializeField]
    [Range(0f, 30f)]
    float grabForce = 10f;
    public LayerMask grabLayer;
    public LayerMask geometryLayer;

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
        _feet = GetComponentInChildren<FootCollider>().GetComponent<Collider>();
    }

    void Update()
    {
        if (currentGrabRB == null)
        {
            Physics.Raycast(_head.position, _head.forward, out var ray, maxGrabDistance, grabLayer);

            if (ray.collider != null) Debug.DrawLine(_head.position, ray.point, Color.green);
            else Debug.DrawLine(_head.position, _head.position + _head.forward * maxGrabDistance, Color.white);

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
            /*
            if (Physics.Raycast(_head.position, _head.forward, out var ray, maxGrabDistance * 10, geometryLayer) &&
                currentGrabRB.GetComponent<Collider>().bounds.Intersects(ray.collider.bounds))
            {
                currentGrabRB.MovePosition(Vector3.Lerp(currentGrabRB.position, _head.position, 0.01f / currentGrabRB.mass)); // Move away
            }
            else
            {
                Vector3 desiredHoldPosition = _head.position + (_head.forward * holdDistance);
                currentGrabRB.MovePosition(Vector3.Lerp(currentGrabRB.position, desiredHoldPosition, 0.1f / currentGrabRB.mass)); // Move towards
            }
            */

            Vector3 posDif = _head.position + (_head.forward * holdDistance) - currentGrabRB.position;
            currentGrabRB.AddForce(posDif * posDif.sqrMagnitude * grabForce);

            if (currentGrabRB.velocity.magnitude > maxGrabSpeed)
                currentGrabRB.velocity = currentGrabRB.velocity.normalized * maxGrabSpeed;

            //armTarget.MoveRotation(Quaternion.Euler(0f, angleX, 0f));

            currentGrabRB.velocity *= 0.9f;
            currentGrabRB.angularVelocity *= 0.9f;
            
        }
    }

    public void ToggleGrab()
    {
        if (!isGrabbing) Grab();
        else Release();
    }

    public bool Grab()
    {
        if (currentLookRB == null) return false;
        currentGrabRB = currentLookRB;

        Physics.IgnoreCollision(_body, currentGrabRB.GetComponent<Collider>(), true);
        Physics.IgnoreCollision(_feet, currentGrabRB.GetComponent<Collider>(), true);
        grabbed?.Invoke(currentGrabRB.transform);
        _grabbing = true;

        if (currentGrabRB.tag == "Box")
        {
            currentGrabRB.freezeRotation = true;
            currentGrabRB.useGravity = false;
        }
        //currentGrabRB.isKinematic = true;
        //currentGrabRB.interpolation = RigidbodyInterpolation.Extrapolate;

        return true;
    }

    public bool Release()
    {
        if (currentGrabRB == null) return false;
        if (currentGrabRB.tag == "Box")
        {
            currentGrabRB.freezeRotation = false;
            currentGrabRB.useGravity = true;
        }
        //currentGrabRB.isKinematic = false;
        //currentGrabRB.interpolation = RigidbodyInterpolation.None;

        StartCoroutine("EnableCollisionsOnExit", currentGrabRB.GetComponent<Collider>());
        letGo?.Invoke(currentGrabRB.transform);
        _grabbing = false;

        currentGrabRB = null;
        return true;
    }

    IEnumerator EnableCollisionsOnExit(Collider grabbedCollider)
    {
        while (_body.bounds.Intersects(grabbedCollider.bounds))
        {
            yield return new WaitForSeconds(0.1f);
        }
        Physics.IgnoreCollision(_body, grabbedCollider.GetComponent<Collider>(), false);
        Physics.IgnoreCollision(_feet, grabbedCollider.GetComponent<Collider>(), false);
    }
}
