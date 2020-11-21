using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ArtificialController : WorkerBase
{
    bool synced = true;
    public int grabFrameBuffer { private get; set; }

    void Start()
    {
        Physics.IgnoreCollision(_body, _feet.GetComponent<Collider>(), true);
    }

    public void UpdateFromRecordedMovement(FrameMovement frameMovement)
    {
        _head.rotation = frameMovement.look;
        if(grabFrameBuffer > 0 || frameMovement.grab)
        {
            if (_grabber.Grab()) grabFrameBuffer = 0;
            else --grabFrameBuffer;
        }
        if (frameMovement.release)
        {
            if(_grabber.Release()) grabFrameBuffer = 0;
        }

        SetAnimatorValues(frameMovement.hMov, frameMovement.vMov);

        /*
        if (frameMovement.jump) _movement.Jump();
        _movement.sprintTime = frameMovement.sprint;
        _movement.ApplyForces(frameMovement.forceNextFrame);

        if (Vector3.Distance(transform.position, frameMovement.position) < 0.1f)
        {
            transform.position = frameMovement.position;
        }
        else
        {
            Debug.Log(name + " desynced!");
        }
        */

        if (synced && frameMovement.jump && !_feet.isGrounded && _movement.jumping)
        {
            //synced = false;
            Debug.Log(name + " desynced!");
        }

        if(synced)
        {
            _movement.Move(frameMovement.position);
        }
        else
        {
            if (frameMovement.jump) _movement.Jump();
            _movement.sprintTime = frameMovement.sprint;
            _movement.ApplyForces(frameMovement.forceNextFrame);
        }

        MovementEvent?.Invoke(frameMovement);
    }
    /*
    // Update is called once per frame
    void Update()
    {
        timeTracker += Time.deltaTime;

        Vector3 newMove = new Vector3(xInput.Evaluate(timeTracker), 0f, yInput.Evaluate(timeTracker));
        _movement.forceNextFrame += newMove * Time.deltaTime;
    }*/
}
