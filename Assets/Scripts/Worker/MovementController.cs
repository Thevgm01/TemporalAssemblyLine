using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    Rigidbody _rb;
    FootCollider _feet;
    Animator _animator;

    [SerializeField]
    [Range(0f, 2f)]
    float walkSpeed = 1f;
    [SerializeField]
    [Range(1f, 3f)]
    float sprintSpeedMult = 2f;
    [SerializeField]
    [Range(0f, 1f)]
    float sprintSpeedChange = 0.5f;
    [SerializeField]
    AnimationCurve airSpeedMult = null;
    //public float maxSpeed;
    [SerializeField]
    [Range(0f, 1f)]
    float speedDecay = 0.15f;
    [SerializeField]
    [Range(0f, 5f)]
    float jumpHeight = 4f;
    [SerializeField]
    [Range(0f, 1f)]
    float floatStrength = 1f;
    bool floating = false;

    [HideInInspector]
    public float sprintTime = 0f;

    public bool jumping { get; private set; }

    private Vector3 lastGroundPosition;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _feet = GetComponentInChildren<FootCollider>();

        _feet.landed += Land;
        jumping = false;
    }

    public void Move(Vector3 destination)
    {
        _rb.MovePosition(destination);
        _rb.velocity = Vector3.zero;
    }

    public Vector3 ApplyForces(Vector3 forceNextFrame)
    {
        lastGroundPosition = _rb.position;

        if (forceNextFrame != Vector3.zero)
        {
            float speed = walkSpeed;

            if (_feet.isGrounded)
            {
                speed *= Mathf.Lerp(1, sprintSpeedMult, sprintTime);
                forceNextFrame *= (1f - speedDecay);
                if (forceNextFrame.sqrMagnitude < 0.0001f) forceNextFrame = Vector3.zero;
                else _rb.MovePosition(_rb.position + forceNextFrame * speed);
            }
            else
            {
                Vector3 currentLateralVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                float dotProduct = Vector3.Dot(currentLateralVelocity.normalized, forceNextFrame.normalized);
                float speedMult = airSpeedMult.Evaluate(currentLateralVelocity.magnitude * dotProduct);
                speed *= speedMult;

                _rb.AddForce(forceNextFrame * speed * 20 / 0.02f);
                forceNextFrame = Vector3.zero;
            }
        }

        if (!_feet.isGrounded && floating)
        {
            _rb.AddForce(new Vector3(0, floatStrength / 0.02f, 0), ForceMode.Acceleration);
            floating = false;
        }

        if (sprintTime > 0f)
        {
            sprintTime -= 0.02f / sprintSpeedChange;
            if (sprintTime < 0f) sprintTime = 0f;
        }

        return forceNextFrame;
    }

    public void Sprint()
    {
        sprintTime += Time.deltaTime * 2 / sprintSpeedChange;
        if (sprintTime > 1f) sprintTime = 1f;
    }

    public void Jump()
    {
        if (jumping || !_feet.isGrounded)
        {
            if (_rb.velocity.y > 0) floating = true;
        }
        else
        {
            Vector3 lateralDelta = new Vector3(_rb.position.x - lastGroundPosition.x, 0, _rb.position.z - lastGroundPosition.z);
            _rb.velocity = new Vector3
                (0, Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), 0) +
                 lateralDelta / 0.02f;
            jumping = true;
        }
    }

    void Land()
    {
        lastGroundPosition = _rb.position;
        jumping = false;
    }
}
