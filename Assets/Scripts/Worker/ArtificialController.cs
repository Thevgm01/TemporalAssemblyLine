using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialController : MonoBehaviour
{
    GrabController _grabber;
    MovementController _movement;
    Transform _head;
    Collider _body;
    FootCollider _feet;
    Animator _animator;

    bool synced = true;
    public int grabFrameBuffer { private get; set; }

    void Awake()
    {
        _grabber = GetComponent<GrabController>();
        _movement = GetComponent<MovementController>();
        _body = GetComponent<Collider>();
        _head = transform.Find("Head");
        _feet = GetComponentInChildren<FootCollider>();
        _animator = GetComponent<Animator>();
    }

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

        if (_animator != null)
        {
            _animator.transform.rotation = Quaternion.Euler(0, _head.rotation.eulerAngles.y, 0);
            _animator.SetFloat("hMov", Mathf.Lerp(_animator.GetFloat("hMov"), frameMovement.hMov, 0.05f));
            _animator.SetFloat("vMov", Mathf.Lerp(_animator.GetFloat("vMov"), frameMovement.vMov, 0.05f));
        }
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

        if(synced && frameMovement.jump && !_feet.isGrounded && _movement.jumping)
        {
            synced = false;
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
