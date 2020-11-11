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
    int numFrames => frameMovements.Count;
    [SerializeField]
    BoxSpawner boxSpawner;
    [SerializeField]
    BoxReceptacle boxReceptacle;

    [SerializeField]
    [Range(0f, 10f)]
    int secondsPerLoop = 3;
    int framesPerLoop;

    [SerializeField]
    [Range(-1, 100)]
    [Tooltip("Negative 1 means unlimited copies")]
    int maxCopies = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (boxSpawner != null) boxSpawner.frequency = 0f;
        if (boxReceptacle != null) boxReceptacle.boxReceived += BoxReceived;

        framesPerLoop = 60 * secondsPerLoop;
    }

    void NewMovement(FrameMovement frameMovement)
    {
        frameMovements.Add(frameMovement);
    }

    void FixedUpdate()
    {
        if (state == State.Idle) return;

        if ((slaveControllers.Count < maxCopies || maxCopies < 0) &&
            numFrames % framesPerLoop == 0 && numFrames > 0)
        {
            ClonePlayer();
            boxSpawner.Spawn();
        }

        for (int i = 0; i < slaveControllers.Count; i++)
        {
            int index = numFrames - (i + 1) * framesPerLoop;
            if (index >= 0)
            {
                if (index >= frameMovements.Count)
                {
                    Destroy(slaveControllers[i].gameObject);
                    slaveControllers.RemoveAt(i);
                }
                else
                {
                    slaveControllers[i].UpdateFromRecordedMovement(frameMovements[index]);
                }
            }
        }
    }

    void ClonePlayer()
    {
        GameObject newSlave = Instantiate(masterController.gameObject);

        Destroy(newSlave.GetComponent<PlayerController>()); // Destroy old player controller
        slaveControllers.Add(newSlave.AddComponent<ArtificialController>()); // Add artificial controller
        newSlave.GetComponent<Animator>().enabled = true; // Enable animator
        newSlave.transform.Find("Worker").gameObject.SetActive(true); // Enable avatar

        newSlave.transform.position = startPosition;
    }

    void BoxReceived(int num)
    {
        if(num > 1 && state == State.Recording)
        {
            masterController.movementEvent -= NewMovement;
            state = State.Looping;
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

            state = State.Recording;
        }
    }
}
