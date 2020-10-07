using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform _camera;
    public float lookSensetivity;

    public float walkSpeed;
    public float runSpeedMult;
    public AnimationCurve airSpeedMult;
    //public float maxSpeed;
    public float speedDecay;
    public float jumpForce;
    private bool jumping = false;

    float angleX = 0f;
    float angleY = 0f;

    private Vector3 curSpeed;

    public GameObject footCollider;
    public HandIconManager handIcon;

    Rigidbody rb;
    FootCollider feet;
    Transform head;
    Collider body;

    [Header("Grabbing")]
    [SerializeField]
    [Range(0f, 5f)]
    float maxGrabDistance = 3f;
    [SerializeField]
    [Range(0f, 5f)]
    float maxGrabSpeed = 2f;
    [SerializeField]
    [Range(0f, 30f)]
    float grabForce = 10f;
    [SerializeField]
    LayerMask grabLayer;
    private float holdDistance;
    private Rigidbody currentGrabbedRB;


    void Awake()
    {
        curSpeed = Vector3.zero;

        rb = GetComponent<Rigidbody>();

        body = GetComponent<Collider>();

        head = transform.Find("Head");

        handIcon.SetState(HandIconManager.State.HIDDEN);

        var feetObject = Instantiate(footCollider);
        var feetFollow = feetObject.GetComponent<FollowObject>();
        feetFollow.target = this.transform;
        feetFollow.position = true;
        feet = feetObject.GetComponent<FootCollider>();
        var feetCollider = feetObject.GetComponent<Collider>();
        Physics.IgnoreCollision(body, feetCollider, true);

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

        HandleGrab();
    }

    void HandleGrab()
    {
        if (currentGrabbedRB == null) {
            Physics.Raycast(_camera.position, _camera.forward, out var ray, maxGrabDistance, grabLayer);
            Rigidbody colliderRB;
            if (ray.collider != null && (colliderRB = ray.collider.GetComponent<Rigidbody>()) != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Grab(colliderRB);
                    holdDistance = ray.distance;
                    handIcon.SetState(HandIconManager.State.CLOSED);
                }
                else
                {
                    handIcon.SetState(HandIconManager.State.OPEN);
                }
            }
            else
            {
                handIcon.SetState(HandIconManager.State.HIDDEN);
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            Release();
            handIcon.SetState(HandIconManager.State.OPEN);
        }
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

        if (currentGrabbedRB != null)
        {
            Vector3 posDif = head.transform.position + (head.transform.forward * holdDistance) - currentGrabbedRB.position;
            currentGrabbedRB.AddForce(posDif * posDif.sqrMagnitude * grabForce);
            currentGrabbedRB.velocity *= 0.9f;

            if (currentGrabbedRB.velocity.magnitude > maxGrabSpeed)
                currentGrabbedRB.velocity = currentGrabbedRB.velocity.normalized * maxGrabSpeed;

            //armTarget.MoveRotation(Quaternion.Euler(0f, angleX, 0f));
            currentGrabbedRB.angularVelocity *= 0.9f;
        }
    }

    void Grab(Rigidbody rb)
    {
        currentGrabbedRB = rb;
        currentGrabbedRB.useGravity = false;
        Physics.IgnoreCollision(body, currentGrabbedRB.GetComponent<Collider>(), true);
    }

    void Release()
    {
        StartCoroutine("EnableCollisionsOnExit", currentGrabbedRB.GetComponent<Collider>());
        currentGrabbedRB.useGravity = true;
        currentGrabbedRB = null;
    }

    IEnumerator EnableCollisionsOnExit(Collider grabbedCollider)
    {
        while (body.bounds.Intersects(grabbedCollider.bounds))
        {
            yield return new WaitForSeconds(0.1f);
        }
        Physics.IgnoreCollision(body, grabbedCollider.GetComponent<Collider>(), false);
    }
}
