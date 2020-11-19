using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRigidbody : MonoBehaviour
{
    public Rigidbody target;

    public bool position;
    public bool rotation;

    void LateUpdate()
    {
        if (position) transform.position = target.position;
        if (rotation) transform.rotation = target.rotation;
    }
}
