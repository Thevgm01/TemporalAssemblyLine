using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementRecorder : MonoBehaviour
{
    private enum State
    {
        Idle,
        Recording,
        Looping
    }
    private State state = State.Idle;

    Action CycleElapsed = delegate { };

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
    float secondsPerCopy = 3;
    int framesPerCopy;

    [SerializeField]
    [Range(-1, 100)]
    [Tooltip("Negative 1 means unlimited copies")]
    int maxCopies = -1;

    public GameObject cloneParticles;
    int framesToSpawnCloneParticles;
    public GameObject deathParticles;

    int grabFrameBuffer = 5;

    // Start is called before the first frame update
    void Awake()
    {
        if (boxSpawner != null)
        {
            boxSpawner.frequency = 0f;
            CycleElapsed += boxSpawner.Spawn;
        }
        if (boxReceptacle != null)
        {
            boxReceptacle.boxReceived += BoxReceived;
        }

        framesPerCopy = (int)(secondsPerCopy / Time.fixedDeltaTime);

        Lever lever = GetComponentInChildren<Lever>();
        if (lever != null) lever.Pulled += Reset;

        var main = cloneParticles.GetComponent<ParticleSystem>().main;
        framesToSpawnCloneParticles = (int)(60 * main.startLifetime.Evaluate(0));
    }

    void NewMovement(FrameMovement frameMovement)
    {
        frameMovements.Add(frameMovement);
    }

    void FixedUpdate()
    {
        if (state == State.Idle) return;

        if ((slaveControllers.Count < maxCopies || maxCopies < 0))
        {
            if (numFrames % framesPerCopy == framesPerCopy - framesToSpawnCloneParticles + 10)
            {
                Instantiate(cloneParticles, startPosition + new Vector3(0, 1, 0), Quaternion.identity);
            }
            else if (numFrames % framesPerCopy == 0 && numFrames > 0)
            {
                ClonePlayer();
                CycleElapsed?.Invoke();
            }
        }

        for (int i = 0; i < slaveControllers.Count; ++i)
        {
            int index = (numFrames % framesPerCopy) + i * framesPerCopy;
            if (index >= frameMovements.Count)
            {
                DestroyLastClone();
            }
            else
            {
                if (index < frameMovements.Count - grabFrameBuffer && frameMovements[index + grabFrameBuffer].grab)
                    slaveControllers[i].grabFrameBuffer = grabFrameBuffer * 2;
                slaveControllers[i].UpdateFromRecordedMovement(frameMovements[index]);
            }
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
        //Instantiate(cloneParticles, startPosition + new Vector3(0, 1, 0), Quaternion.identity);
    }

    void DestroyLastClone()
    {
        ArtificialController lastController = slaveControllers[slaveControllers.Count - 1];
        Instantiate(deathParticles, lastController.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        lastController.UpdateFromRecordedMovement(new FrameMovement { release = true });
        Destroy(lastController.gameObject);
        slaveControllers.RemoveAt(slaveControllers.Count - 1);
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
            //DestroyLastClone();
        }
    }

    void Reset()
    {
        while (slaveControllers.Count > 0) DestroyLastClone();
        state = State.Idle;
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
            CycleElapsed?.Invoke();

            numFrames = 0;
            state = State.Recording;
        }
    }
}
