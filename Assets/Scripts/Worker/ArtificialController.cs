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

    void Awake()
    {
        _grabber = GetComponent<GrabController>();
        _movement = GetComponent<MovementController>();
        _body = GetComponent<Collider>();
        _head = transform.Find("Head");
        _feet = GetComponentInChildren<FootCollider>();
        _animator = GetComponent<Animator>();

        Physics.IgnoreCollision(_body, _feet.GetComponent<Collider>(), true);
    }

    public void UpdateFromRecordedMovement(FrameMovement frameMovement)
    {
        _head.rotation = frameMovement.look;
        if (frameMovement.vMov != 0 || frameMovement.hMov != 0)
        {
            float faceAngle = Mathf.Atan2(frameMovement.hMov, frameMovement.vMov) * Mathf.Rad2Deg + _head.rotation.eulerAngles.y;
            Vector3 newMove = Quaternion.Euler(0f, faceAngle, 0f) * Vector3.forward;
            _movement.forceNextFrame += newMove * Time.deltaTime;
        }
        if (frameMovement.jump) _movement.Jump();
        if (frameMovement.sprint) _movement.Sprint();
        if (frameMovement.grab) _grabber.ToggleGrab();

        if (_animator != null)
        {
            _animator.transform.rotation = Quaternion.Euler(0, _head.rotation.eulerAngles.y, 0);
            _animator.SetFloat("hMov", Mathf.Lerp(_animator.GetFloat("hMov"), frameMovement.hMov, 0.05f));
            _animator.SetFloat("vMov", Mathf.Lerp(_animator.GetFloat("vMov"), frameMovement.vMov, 0.05f));
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
