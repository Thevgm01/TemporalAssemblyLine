using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openHeight = 4.9f;

    ConfigurableJoint joint;
    public Lever lever;

    // Start is called before the first frame update
    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();

        if(lever != null) lever.Pulled += Open;
    }

    // Update is called once per frame
    void Open()
    {
        joint.anchor = new Vector3(0, openHeight, 0);
        joint.connectedBody.AddForce(Vector3.one / 1000f);
    }
}
