using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    public Action Opened = delegate { };

    float closeHeight;
    public float openHeight = 4.9f;

    ConfigurableJoint joint;
    public Lever lever;

    [SerializeField]
    int _numBoxesToUnlock;
    int NumBoxes
    {
        get => _numBoxesToUnlock;
        set
        {
            _numBoxesToUnlock = value;
            ChangeBoxes();
        }
    }
    bool Locked { get { return _numBoxesToUnlock > 0; } }
    int startNumBoxes;

    public BoxReceptacle boxReceptacle;
    TMPro.TextMeshPro doorText;

    // Start is called before the first frame update
    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        closeHeight = joint.anchor.y;

        doorText = transform.GetChild(0).GetComponentInChildren<TMPro.TextMeshPro>();

        if (lever == null) Debug.LogWarning("Door missing lever");
        else lever.Pulled += Open;

        if(NumBoxes > 0)
        {
            if (boxReceptacle == null) Debug.LogError("Locked door missing box receptacle");
            boxReceptacle.boxReceived += BoxDeposit;

            startNumBoxes = NumBoxes;
            ChangeBoxes();
        }
    }

    void Open()
    {
        if (Locked) return;

        joint.anchor = new Vector3(0, openHeight, 0);
        joint.connectedBody.AddForce(Vector3.one / 1000f);
        Opened?.Invoke();
    }

    public void Close()
    {
        if (Locked) return;

        NumBoxes = startNumBoxes;

        joint.anchor = new Vector3(0, closeHeight, 0);
        joint.connectedBody.AddForce(Vector3.one / 1000f);
        //Closed?.Invoke();
    }

    void BoxDeposit(int num)
    {
        --NumBoxes;
    }

    public void ResetCount()
    {
        if(Locked) NumBoxes = startNumBoxes;
    }

    void ChangeBoxes()
    {
        if(Locked)
        {
            doorText.text = "Locked\n" + NumBoxes + (NumBoxes > 1 ? " boxes" : " box") + "\nremaining";
        }
        else
        {
            doorText.text = "Unlocked";
        }
    }
}
