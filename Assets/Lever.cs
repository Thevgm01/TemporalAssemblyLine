using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lever : MonoBehaviour
{
    public Action Pulled = delegate { };

    private HingeJoint hinge;
    bool pulled = false;

    int activateMargin = 5, deactivateMargin = 20;

    void Awake()
    {
        hinge = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!pulled && hinge.angle >= hinge.limits.max - activateMargin)
        {
            Pulled?.Invoke();
            pulled = true;
        }
        else if(pulled && hinge.angle < hinge.limits.max - deactivateMargin)
        {
            pulled = false;
        }
    }
}
