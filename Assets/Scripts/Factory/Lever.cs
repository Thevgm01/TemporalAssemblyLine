using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Lever : MonoBehaviour
{
    [SerializeField] AudioClip pullSound;
    [SerializeField] AudioClip releaseSound;

    public Action Pulled = delegate { };
    public Action Released = delegate { };

    private HingeJoint hinge;
    bool pulled = false;

    float activateMargin, deactivateMargin;

    void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        float diff = hinge.limits.max - hinge.limits.min;
        activateMargin = hinge.limits.min + diff * 0.67f;
        deactivateMargin = hinge.limits.min + diff * 0.33f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!pulled && hinge.angle >= activateMargin)
        {
            Pulled?.Invoke();
            pulled = true;
            AudioHelper.PlayClip(pullSound, 0.3f, 1f, transform);
        }
        else if(pulled && hinge.angle < deactivateMargin)
        {
            Released?.Invoke();
            pulled = false;
            AudioHelper.PlayClip(releaseSound, 0.3f, 1f, transform);
        }
    }
}
