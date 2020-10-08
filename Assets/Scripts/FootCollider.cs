using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FootCollider : MonoBehaviour
{
    private int _grounded = 0;

    public bool isGrounded
    {
        get => _grounded > 0;
    }

    void OnTriggerEnter(Collider c)
    {
        _grounded++;
    }

    void OnTriggerExit(Collider c)
    {
        _grounded--;
    }

}
