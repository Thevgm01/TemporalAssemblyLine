using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct FrameMovement
{
    public Quaternion look;
    public float hMov;
    public float vMov;
    public bool sprint;
    public bool jump;
    public bool grab;
}

public class PlayerController : MonoBehaviour
{
    public Action<FrameMovement> movementEvent;

    public Transform _camera;
    GrabController _grabber;
    MovementController _movement;
    Transform _head;
    Collider _body;
    FootCollider _feet;
    Animator _animator;

    float lookAngleX = 0f;
    float lookAngleY = 0f;
    [SerializeField]
    [Range(0f, 5f)]
    public float lookSensetivity;

    public HandIconManager handIcon;

    void Awake()
    {
        _grabber = GetComponent<GrabController>();
        _movement = GetComponent<MovementController>();
        _body = GetComponent<Collider>();
        _head = transform.Find("Head");
        _feet = GetComponentInChildren<FootCollider>();
        _animator = GetComponent<Animator>();

        Physics.IgnoreCollision(_body, _feet.GetComponent<Collider>(), true);

        handIcon.Hide();
        _grabber.cannotGrab += handIcon.Hide;
        _grabber.canGrab += handIcon.Open;
        _grabber.grabbed += handIcon.Close;
        _grabber.letGo += handIcon.Open;

        Cursor.lockState = CursorLockMode.Locked;

    }

    void Update()
    {
        FrameMovement frameMovement = new FrameMovement();

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
            _movement.forceNextFrame += newMove * Time.deltaTime;

            frameMovement.hMov = hMov;
            frameMovement.vMov = vMov;
        }

        if (_animator != null)
        {
            _animator.transform.rotation = Quaternion.Euler(0, _head.rotation.eulerAngles.y, 0);
            _animator.SetFloat("vMov", Mathf.Lerp(_animator.GetFloat("vMov"), vMov, 0.05f));
            _animator.SetFloat("hMov", Mathf.Lerp(_animator.GetFloat("hMov"), hMov, 0.05f));
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _movement.Sprint();
            frameMovement.sprint = true;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _movement.Jump();
            frameMovement.jump = true;
        }

        if(Input.GetMouseButtonDown(0))
        {
            _grabber.ToggleGrab();
            frameMovement.grab = true;
        }

        movementEvent?.Invoke(frameMovement);
    }
}
