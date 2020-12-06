using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaMovementRecorder : MovementRecorder
{
    Door[] allDoors;
    public MeshRenderer[] meshesToDisable;
    public GameObject quitMessage;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        if (endDoor != null)
        {
            endDoor.Opened -= ResetAll;
            endDoor.Opened += BeginLooping;
        }

        framesPerCopy = int.MaxValue;
        CalculateSpawnParticlesOffset();

        allDoors = FindObjectsOfType<Door>();
    }

    void BeginLooping()
    {
        framesPerCopy = numFrames;
        CalculateSpawnParticlesOffset();

        foreach (Door door in allDoors)
        {
            if (door != endDoor) door.Close();
        }

        foreach (MeshRenderer mr in meshesToDisable)
        {
            if (mr) mr.enabled = false;
        }

        quitMessage.SetActive(true);
    }
}
