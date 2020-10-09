using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform _camera;
    GrabController _grabber;
    MovementController _movement;
    Transform _head;
    Collider _body;
    FootCollider _feet;

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
        // Looking
        lookAngleX = Mathf.Repeat(lookAngleX + Input.GetAxis("Mouse X") * lookSensetivity, 360f);
        lookAngleY = Mathf.Clamp(lookAngleY - Input.GetAxis("Mouse Y") * lookSensetivity, -90f, 90f);

        float vMov = Input.GetAxisRaw("Vertical"), hMov = Input.GetAxisRaw("Horizontal");
        if (vMov != 0 || hMov != 0)
        {
            float faceAngle = Mathf.Atan2(hMov, vMov) * Mathf.Rad2Deg + lookAngleX;
            Vector3 newMove = Quaternion.Euler(0f, faceAngle, 0f) * Vector3.forward;
            _movement.forceNextFrame += newMove * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _movement.Sprint(true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _movement.Sprint(false);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (_feet.isGrounded) _movement.Jump();
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (!_grabber.isGrabbing) _grabber.Grab();
            else _grabber.Release();
        }
    }

    void LateUpdate()
    {
        _head.rotation = Quaternion.Euler(lookAngleY, lookAngleX, 0);
    }
}
