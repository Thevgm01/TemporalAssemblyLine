using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollider : MonoBehaviour
{
    int grounded = 0;

    public bool Grounded()
    {
        return grounded > 0;
    }

    void OnTriggerEnter(Collider c)
    {
        grounded++;
    }

    void OnTriggerExit(Collider c)
    {
        grounded--;
    }

}
