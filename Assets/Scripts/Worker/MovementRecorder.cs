using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct FrameMovement
{
    public Quaternion look;
    public Vector3 forceNextFrame;
    public Vector3 position;
    public float hMov;
    public float vMov;
    public float sprint;
    public bool jump;
    public bool grab;
    public bool release;
}

public class MovementRecorder : MonoBehaviour
{
    [SerializeField] AudioClip activateSound;
    [SerializeField] AudioClip resetSound;
    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip destroySound;

    private enum State
    {
        Idle,
        Recording,
        Looping
    }
    private State state = State.Idle;

    Action CycleElapsed = delegate { };
    Action Reset = delegate { };

    WorkerBase masterController;
    Vector3 startPosition;
    List<ArtificialController> slaveControllers;
    List<FrameMovement> frameMovements;
    protected int numFrames;
    [SerializeField] BoxSpawner boxSpawner = null;
    [SerializeField] BoxReceptacle boxReceptacle = null;
    [SerializeField] protected Door endDoor = null;

    [SerializeField]
    [Range(0f, 10f)]
    float secondsPerCopy = 3;
    protected int framesPerCopy;

    [SerializeField]
    [Range(-1, 100)]
    [Tooltip("Negative 1 means unlimited copies")]
    int maxCopies = -1;

    //public GameObject cloneModel;
    [SerializeField] GameObject cloneParticles;
    int framesToSpawnCloneParticles;
    [SerializeField] GameObject deathParticles;

    int grabFrameBuffer = 5;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (boxSpawner != null)
        {
            boxSpawner.frequency = 0f;
            CycleElapsed += boxSpawner.Spawn;
            Reset += boxSpawner.DestroyAll;
        }
        if (boxReceptacle != null)
        {
            boxReceptacle.boxReceived += BoxReceived;
        }
        if (endDoor != null)
        {
            endDoor.Opened += ResetAll;
            Reset += endDoor.ResetCount;
        }

        Lever lever = GetComponentInChildren<Lever>();
        if (lever != null) lever.Pulled += ResetAll;

        framesPerCopy = (int)(secondsPerCopy / Time.fixedDeltaTime);
        CalculateSpawnParticlesOffset();
    }

    protected void CalculateSpawnParticlesOffset()
    {
        var main = cloneParticles.GetComponent<ParticleSystem>().main;
        framesToSpawnCloneParticles = framesPerCopy - (int)(60 * main.startLifetime.Evaluate(0)) + 10;
    }

    void NewMovement(FrameMovement frameMovement)
    {
        frameMovements.Add(frameMovement);
    }

    void FixedUpdate()
    {
        if (state == State.Idle) return;

        if (numFrames % framesPerCopy == framesToSpawnCloneParticles)
        {
            Instantiate(cloneParticles, startPosition + new Vector3(0, 1, 0), Quaternion.identity);
            AudioHelper.PlayClip(spawnSound, 0.3f, 1f, transform);
        }
        else if (numFrames % framesPerCopy == 0 && numFrames > 0)
        {
            if (slaveControllers.Count < maxCopies || maxCopies < 0)
            {
                ClonePlayer();
                CycleElapsed?.Invoke();
            }
            else
            {
                ResetAll();
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

        Destroy(newSlave.GetComponent<WorkerBase>()); // Destroy old controller
        slaveControllers.Insert(0, newSlave.AddComponent<ArtificialController>()); // Add artificial controller

        newSlave.GetComponent<Animator>().enabled = true; // Enable animator

        var avatar = newSlave.transform.Find("Worker");
        var skin = avatar.GetComponentInChildren<SkinnedMeshRenderer>();
        skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        skin.receiveShadows = true;

        newSlave.transform.position = startPosition;
        //Instantiate(cloneParticles, startPosition + new Vector3(0, 1, 0), Quaternion.identity);
    }

    void DestroyLastClone()
    {
        ArtificialController lastController = slaveControllers[slaveControllers.Count - 1];
        Instantiate(deathParticles, lastController.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        AudioHelper.PlayClip(destroySound, 1f, 1f, lastController.transform);
        lastController.UpdateFromRecordedMovement(new FrameMovement { release = true });
        Destroy(lastController.gameObject);
        slaveControllers.RemoveAt(slaveControllers.Count - 1);
    }

    void BoxReceived(int num)
    {
        if(num > 0 && state == State.Recording)
        {
            masterController.MovementEvent -= NewMovement;
            state = State.Looping;
        }
        else if(state == State.Looping)
        {
            //DestroyLastClone();
        }
    }

    protected void ResetAll()
    {
        if (slaveControllers == null) return;

        if (state == State.Recording) masterController.MovementEvent -= NewMovement;
        if (state != State.Idle) AudioHelper.PlayClip(resetSound, 1f, 1f, transform);

        while (slaveControllers.Count > 0) DestroyLastClone();
        frameMovements.Clear();
        state = State.Idle;

        Reset?.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        if (state == State.Idle && other.tag == "Player")
        {
            masterController = other.GetComponent<WorkerBase>();
            masterController.MovementEvent += NewMovement;

            startPosition = other.transform.position;

            slaveControllers = new List<ArtificialController>();
            frameMovements = new List<FrameMovement>();
            CycleElapsed?.Invoke();

            numFrames = 1;
            state = State.Recording;

            AudioHelper.PlayClip(activateSound, 1f, 1f, transform);
        }
    }
}
