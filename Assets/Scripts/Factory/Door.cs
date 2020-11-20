using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openHeight = 4.9f;

    bool locked = false;

    ConfigurableJoint joint;
    public Lever lever;

    [SerializeField]
    int _numBoxesToUnlock;
    int NumBoxes
    {
        get { return _numBoxesToUnlock; }
        set
        {
            _numBoxesToUnlock = value;
            ChangeBoxes();
        }
    }
    int startNumBoxes;
    public BoxReceptacle boxReceptacle;
    TMPro.TextMeshPro doorText;

    // Start is called before the first frame update
    void Awake()
    {
        joint = GetComponent<ConfigurableJoint>();
        doorText = transform.GetChild(0).GetComponentInChildren<TMPro.TextMeshPro>();

        if (lever == null) Debug.LogWarning("Door missing lever");
        else lever.Pulled += Open;

        if(NumBoxes > 0)
        {
            if (boxReceptacle == null) Debug.LogError("Locked door missing box receptacle");
            boxReceptacle.boxReceived += BoxDeposit;

            startNumBoxes = NumBoxes;
            locked = true;
            ChangeBoxes();
        }
    }

    void Open()
    {
        if (locked)
        {
            return;
        }

        joint.anchor = new Vector3(0, openHeight, 0);
        joint.connectedBody.AddForce(Vector3.one / 1000f);
    }

    void BoxDeposit(int num)
    {
        --NumBoxes;
    }

    void Reset()
    {
        if(startNumBoxes > 0)
        {
            NumBoxes = startNumBoxes;
            locked = true;
        }
    }

    void ChangeBoxes()
    {
        if(NumBoxes > 0)
        {
            doorText.text = "Locked\n" + NumBoxes + (NumBoxes > 1 ? " boxes" : " box") + "\nremaining";
        }
        else
        {
            doorText.text = "Unlocked";
            locked = false;
        }
    }
}
