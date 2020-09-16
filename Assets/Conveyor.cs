using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public bool active;
    public float speed;

    Rigidbody rb;
    Material mat;

    private float tile = 4;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            mat.mainTextureOffset -= new Vector2(0f, speed * Time.fixedDeltaTime / tile);
        }
    }

    void OnCollisionStay(Collision c)
    {
        Vector3 movement = -transform.forward * speed * Time.deltaTime;

        rb.position -= movement;
        rb.MovePosition(rb.position + movement);
    }
}
