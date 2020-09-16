using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float lookSensetivity;

    public float walkSpeed;
    public float runSpeedMult;
    public AnimationCurve airSpeedMult;
    //public float maxSpeed;
    public float speedDecay;
    public float jumpForce;

    float angleX = 0f;
    float angleY = 0f;

    private Vector3 curSpeed;

    public GameObject footCollider;
    public HandIconManager handIcon;

    Rigidbody rb;
    FootCollider feet;
    Transform head;
    Collider body;
    Collider arm;
    List<Collider> armTargets;
    Rigidbody armTarget;

    private bool jumping = false;

    // Use this for initialization
    void Start()
    {
        curSpeed = Vector3.zero;

        rb = GetComponent<Rigidbody>();

        body = GetComponent<Collider>();

        foreach (Transform child in transform)
        {
            if (child.tag == "PlayerHead")
            {
                head = child;
                foreach (Transform subchild in head.transform)
                {
                    if (subchild.tag == "PlayerArm")
                    {
                        arm = subchild.GetComponent<Collider>();
                        break;
                    }
                }
                break;
            }
        }
        armTargets = new List<Collider>();

        var feetObject = Instantiate(footCollider);
        var feetFollow = feetObject.GetComponent<FollowObject>();
        feetFollow.target = this.transform;
        feetFollow.position = true;
        feet = feetObject.GetComponent<FootCollider>();
        var feetCollider = feetObject.GetComponent<Collider>();
        Physics.IgnoreCollision(body, feetCollider, true);
        Physics.IgnoreCollision(arm, feetCollider, true);

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Looking
        angleX += Input.GetAxis("Mouse X") * lookSensetivity;
        angleY -= Input.GetAxis("Mouse Y") * lookSensetivity;
        angleY = Mathf.Clamp(angleY, -90f, 90f);
        head.rotation = Quaternion.Euler(angleY, angleX, 0);

        bool grounded = feet.Grounded();

        float moveSpeed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && grounded) moveSpeed *= runSpeedMult;
        if (!grounded) moveSpeed *= airSpeedMult.Evaluate(Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z));
        else if (Input.GetKeyDown(KeyCode.Space)) jumping = true;

        float vMov = Input.GetAxis("Vertical"), hMov = Input.GetAxis("Horizontal");
        if (vMov != 0 || hMov != 0)
        {
            float aCos = Mathf.Cos(angleX * Mathf.Deg2Rad), aSin = Mathf.Sin(angleX * Mathf.Deg2Rad);
            Vector3 newMove = new Vector3(aSin * vMov + aCos * hMov, 0f, aCos * vMov - aSin * hMov).normalized;
            newMove *= moveSpeed * Time.deltaTime;
            curSpeed += newMove;
        }

        // Grabbing
        if (Input.GetMouseButtonDown(0))
        {
            if (armTarget == null)
            {
                Rigidbody closest = null;
                foreach (Collider c in armTargets)
                {
                    var rb = c.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic)
                    {
                        if (closest == null || Vector3.Distance(head.position, rb.transform.position) < Vector3.Distance(head.position, closest.transform.position))
                            closest = rb;
                    }
                }
                if (closest != null)
                {
                    Grab(closest);
                }
            }
            else if ((armTarget.position - transform.position).sqrMagnitude > 1)
            {
                Drop();
            }
        }

        if (armTarget != null) handIcon.Close();
        else if (armTargets.Count > 0) handIcon.Open();
        else handIcon.Hide();
    }

    void FixedUpdate()
    {
        bool grounded = feet.Grounded();

        if (grounded)
        {
            if (jumping)
            {
                rb.velocity = new Vector3(rb.velocity.x + curSpeed.x * 40, jumpForce, rb.velocity.z + curSpeed.z * 40);
                curSpeed = Vector3.zero;
                jumping = false;
            }
            else
            {
                curSpeed *= (1f - speedDecay);
                if (curSpeed.sqrMagnitude < 0.00001f) curSpeed = Vector3.zero;
                else rb.MovePosition(transform.position + curSpeed);
            }
        }
        else if (curSpeed != Vector3.zero)
        {
            rb.AddForce(curSpeed * 2000);
            curSpeed = Vector3.zero;
        }

        if (armTarget != null)
        {
            Vector3 posDif = arm.transform.position - armTarget.position;
            armTarget.AddForce(posDif * (posDif.sqrMagnitude + 1f) * 10f);
            armTarget.velocity *= 0.9f;

            //armTarget.MoveRotation(Quaternion.Euler(0f, angleX, 0f));
            armTarget.angularVelocity = Vector3.zero;
        }
    }

    void Grab(Rigidbody rb)
    {
        armTarget = rb;

        armTarget.useGravity = false;
        Physics.IgnoreCollision(body, armTarget.GetComponent<Collider>(), true);
    }

    void Drop()
    {
        armTarget.useGravity = true;
        Physics.IgnoreCollision(body, armTarget.GetComponent<Collider>(), false);

        armTarget = null;
    }

    void OnTriggerEnter(Collider c) { if (c.tag == "Grabbable") armTargets.Add(c); }
    void OnTriggerExit(Collider c) { armTargets.Remove(c); }
}
