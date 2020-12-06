using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Door : MonoBehaviour
{
    [SerializeField] AudioClip unlockProgressSound;
    [SerializeField] AudioClip unlockSound;
    [SerializeField] AudioClip resetSound;
    [SerializeField] AudioClip openSound;
    [SerializeField] AudioClip closeSound;

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
            if (_numBoxesToUnlock == 0) Unlock();
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
        AudioHelper.PlayClip(openSound, 1f, 1f, transform);
    }

    public void Close()
    {
        if (Locked) return;

        NumBoxes = startNumBoxes;

        joint.anchor = new Vector3(0, closeHeight, 0);
        joint.connectedBody.AddForce(Vector3.one / 1000f);
        //Closed?.Invoke();
        AudioHelper.PlayClip(closeSound, 1f, 1f, transform);
    }

    void Unlock()
    {
        AudioHelper.PlayClip(unlockSound);
    }

    void BoxDeposit(int num)
    {
        --NumBoxes;
    }

    public void ResetCount()
    {
        if (Locked && NumBoxes != startNumBoxes)
        {
            NumBoxes = startNumBoxes;
            AudioHelper.PlayClip(resetSound);
        }
    }

    void ChangeBoxes()
    {
        if(Locked)
        {
            doorText.text = "Locked\n" + NumBoxes + (NumBoxes > 1 ? " boxes" : " box") + "\nremaining";
            AudioHelper.PlayClip(unlockProgressSound, 0.5f, Mathf.Lerp(0.8f, 0.2f, (float)(NumBoxes - 1) / startNumBoxes));
        }
        else
        {
            doorText.text = "Unlocked";
        }
    }
}
