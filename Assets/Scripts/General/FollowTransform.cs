using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform target;

    public bool position;
    public bool rotation;

    // Update is called once per frame
    void LateUpdate()
    {
        if (position) transform.position = target.transform.position;
        if (rotation) transform.rotation = target.transform.rotation;
    }
}
