using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRecorder : MonoBehaviour
{
    public PlayerController masterController;
    public List<ArtificialController> slaveControllers;

    // Start is called before the first frame update
    void Start()
    {
        masterController.movementEvent += NewMovement;
    }

    void NewMovement(FrameMovement frameMovement)
    {
        foreach(var controller in slaveControllers)
        {
            controller.UpdateFromRecordedMovement(frameMovement);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
