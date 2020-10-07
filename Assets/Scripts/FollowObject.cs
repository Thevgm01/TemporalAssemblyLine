using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Transform target;

    public bool position;
    public bool rotation;

    // Update is called once per frame
    void LateUpdate()
    {
        if (position) this.transform.position = target.transform.position;
        if (rotation) this.transform.rotation = target.transform.rotation;
    }
}
