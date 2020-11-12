using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRecorder : MonoBehaviour
{
    private enum State
    {
        Idle,
        Recording,
        Looping
    }
    private State state = State.Idle;

    PlayerController masterController;
    Vector3 startPosition;
    List<ArtificialController> slaveControllers;
    List<FrameMovement> frameMovements;
    int numFrames;
    [SerializeField]
    BoxSpawner boxSpawner = null;
    [SerializeField]
    BoxReceptacle boxReceptacle = null;

    [SerializeField]
    [Range(0f, 10f)]
    int secondsPerCopy = 3;
    int framesPerCopy;

    [SerializeField]
    [Range(-1, 100)]
    [Tooltip("Negative 1 means unlimited copies")]
    int maxCopies = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (boxSpawner != null) boxSpawner.frequency = 0f;
        if (boxReceptacle != null) boxReceptacle.boxReceived += BoxReceived;

        framesPerCopy = (int)(secondsPerCopy / Time.fixedDeltaTime);
    }

    void NewMovement(FrameMovement frameMovement)
    {
        frameMovements.Add(frameMovement);
    }

    void FixedUpdate()
    {
        if (state == State.Idle) return;

        if ((slaveControllers.Count < maxCopies || maxCopies < 0) &&
            numFrames % framesPerCopy == 0 && numFrames > 0)
        {
            ClonePlayer();
            boxSpawner.Spawn();
        }

        for (int i = 0; i < slaveControllers.Count; ++i)
        {
            int index = (numFrames % framesPerCopy) + i * framesPerCopy;
            //if (index >= frameMovements.Count) { }
            //else
            //{
                slaveControllers[i].UpdateFromRecordedMovement(frameMovements[index]);
            //}
        }

        ++numFrames;
    }

    void ClonePlayer()
    {
        GameObject newSlave = Instantiate(masterController.gameObject);
        newSlave.name = "PlayerClone" + numFrames;

        Destroy(newSlave.GetComponent<PlayerController>()); // Destroy old player controller
        slaveControllers.Insert(0, newSlave.AddComponent<ArtificialController>()); // Add artificial controller
        newSlave.GetComponent<Animator>().enabled = true; // Enable animator
        newSlave.transform.Find("Worker").gameObject.SetActive(true); // Enable avatar

        newSlave.transform.position = startPosition;
    }

    void BoxReceived(int num)
    {
        if(num > 0 && state == State.Recording)
        {
            masterController.movementEvent -= NewMovement;
            state = State.Looping;
        }
        else if(state == State.Looping)
        {
            Destroy(slaveControllers[slaveControllers.Count - 1].gameObject);
            slaveControllers.RemoveAt(slaveControllers.Count - 1);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(state == State.Idle && other.tag == "Player")
        {
            masterController = other.GetComponent<PlayerController>();
            masterController.movementEvent += NewMovement;

            startPosition = other.transform.position;

            slaveControllers = new List<ArtificialController>();
            frameMovements = new List<FrameMovement>();
            boxSpawner.Spawn();

            numFrames = 0;
            state = State.Recording;
        }
    }
}
