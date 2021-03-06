﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : WorkerBase
{
    [SerializeField] AudioClip[] footstepSounds;

    FrameMovement frameMovement;

    public Transform _camera;

    float lookAngleX = 0f;
    float lookAngleY = 0f;
    [SerializeField]
    [Range(0f, 5f)]
    public float lookSensetivity;

    public HandIconManager handIcon;

    float totalMovement = 0;
    [SerializeField] float distancePerStep = 1;

    void Start()
    {
        handIcon.Hide();
        _grabber.cannotGrab += handIcon.Hide;
        _grabber.canGrab += handIcon.Open;
        _grabber.grabbed += handIcon.Close;
        _grabber.grabbed += x => { frameMovement.grab = true; };
        _grabber.letGo += handIcon.Open;
        _grabber.letGo += x => { frameMovement.release = true; };

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Physics.IgnoreCollision(_body, _feet.GetComponent<Collider>(), true);
    }

    void Update()
    {
        // Looking
        lookAngleX = Mathf.Repeat(lookAngleX + Input.GetAxis("Mouse X") * lookSensetivity, 360f);
        lookAngleY = Mathf.Clamp(lookAngleY - Input.GetAxis("Mouse Y") * lookSensetivity, -90f, 90f);
        _head.rotation = Quaternion.Euler(lookAngleY, lookAngleX, 0);
        frameMovement.look = _head.rotation;

        float vMov = Input.GetAxisRaw("Vertical"), hMov = Input.GetAxisRaw("Horizontal");
        if (vMov != 0 || hMov != 0)
        {
            float faceAngle = Mathf.Atan2(hMov, vMov) * Mathf.Rad2Deg + lookAngleX;
            Vector3 newMove = Quaternion.Euler(0f, faceAngle, 0f) * Vector3.forward;
            //_movement.forceNextFrame += newMove * Time.deltaTime;

            frameMovement.hMov += hMov;
            frameMovement.vMov += vMov;
            frameMovement.forceNextFrame += newMove * Time.deltaTime;

            totalMovement += newMove.magnitude * Time.deltaTime;
            if(_feet.isGrounded && totalMovement >= distancePerStep)
            {
                AudioHelper.PlayRandomClipFromArray(footstepSounds, 0.2f, 1f);
                totalMovement = 0;
            }
        }

        SetAnimatorValues(hMov, vMov);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _movement.Sprint();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _movement.Jump();
            frameMovement.jump = true;
        }

        if(Input.GetMouseButtonDown(0))
        {
            _grabber.Grab();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            _grabber.Release();
        }
    }

    void FixedUpdate()
    {
        frameMovement.sprint = _movement.sprintTime;
        Vector3 tempForce = _movement.ApplyForces(frameMovement.forceNextFrame);
        MovementEvent?.Invoke(frameMovement);
        frameMovement = new FrameMovement();
        frameMovement.position = transform.position;
        frameMovement.forceNextFrame = tempForce;
    }

    void LateUpdate()
    {
        _camera.position = Vector3.Lerp(_camera.position, _head.position, 0.5f);
        _camera.rotation = _head.rotation;
    }
}
