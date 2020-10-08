using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody _rb;
    FootCollider _feet;

    [SerializeField]
    [Range(0f, 2f)]
    float walkSpeed = 1f;
    [SerializeField]
    [Range(1f, 3f)]
    float sprintSpeedMult = 2f;
    [SerializeField]
    AnimationCurve airSpeedMult;
    //public float maxSpeed;
    [SerializeField]
    [Range(0f, 1f)]
    public float speedDecay = 0.15f;
    [SerializeField]
    [Range(0f, 5f)]
    public float jumpHeight = 4f;

    private bool jumping = false;
    private bool sprinting = false;

    [HideInInspector]
    public Vector3 forceNextFrame;

    void Awake()
    {
        forceNextFrame = Vector3.zero;

        _rb = GetComponent<Rigidbody>();
        _feet = GetComponentInChildren<FootCollider>();
    }

    void FixedUpdate()
    {
        if (forceNextFrame != Vector3.zero)
        {
            float speed = walkSpeed;

            if (_feet.isGrounded)
            {
                if (sprinting) speed *= sprintSpeedMult;
                forceNextFrame *= (1f - speedDecay);
                if (forceNextFrame.sqrMagnitude < 0.0001f) forceNextFrame = Vector3.zero;
                else _rb.MovePosition(transform.position + forceNextFrame * speed);
            }
            else
            {
                Vector3 currentLateralVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                speed *= airSpeedMult.Evaluate(currentLateralVelocity.magnitude);

                _rb.AddForce(forceNextFrame * speed * 1000);
                forceNextFrame = Vector3.zero;
            }
        }
    }

    public void Sprint(bool val)
    {
        sprinting = val;
    }

    public void Jump()
    {
        if (!_feet.isGrounded) return;
        _rb.velocity = new Vector3(_rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), _rb.velocity.z);
    }
}
