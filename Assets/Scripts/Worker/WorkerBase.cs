using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorkerBase : MonoBehaviour
{
    public Action<FrameMovement> MovementEvent = delegate { };

    protected Rigidbody _rb;
    protected GrabController _grabber;
    protected MovementController _movement;
    protected Transform _head;
    protected Collider _body;
    protected FootCollider _feet;
    protected Animator _animator;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _grabber = GetComponent<GrabController>();
        _movement = GetComponent<MovementController>();
        _body = GetComponent<Collider>();
        _head = transform.Find("Head");
        _feet = GetComponentInChildren<FootCollider>();
        _animator = GetComponent<Animator>();
    }

    protected void SetAnimatorValues(float hMov, float vMov)
    {
        if (_animator != null)
        {
            _animator.transform.rotation = Quaternion.Euler(0, _head.rotation.eulerAngles.y, 0);
            _animator.SetFloat("hMov", Mathf.Lerp(_animator.GetFloat("hMov"), hMov, 0.05f));
            _animator.SetFloat("vMov", Mathf.Lerp(_animator.GetFloat("vMov"), vMov, 0.05f));
            _animator.SetFloat("verticalSpeed", Mathf.Abs(_rb.velocity.y));
        }
    }
}
