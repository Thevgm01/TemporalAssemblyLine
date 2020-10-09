using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class FootCollider : MonoBehaviour
{
    public Action landed = delegate { };

    private int _grounded = 0;

    public bool isGrounded
    {
        get => _grounded > 0;
    }

    void OnTriggerEnter(Collider c)
    {
        _grounded++;
        if (isGrounded) landed?.Invoke();
    }

    void OnTriggerExit(Collider c)
    {
        _grounded--;
    }

}
