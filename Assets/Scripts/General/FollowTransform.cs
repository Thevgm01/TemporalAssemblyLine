using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;

    public bool position;
    public bool rotation;

    void LateUpdate()
    {
        if (position) transform.position = target.position;
        if (rotation) transform.rotation = target.rotation;
    }
}
