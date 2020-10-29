using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRecorder : MonoBehaviour
{
    PlayerController masterController;
    Vector3 startPosition;
    List<ArtificialController> slaveControllers;
    List<FrameMovement> frameMovements;
    [SerializeField]
    BoxSpawner boxSpawner;

    [SerializeField]
    [Range(0f, 10f)]
    int secondsPerLoop = 3;
    int framesPerLoop;

    int startFrame;


    // Start is called before the first frame update
    void Start()
    {
        if (boxSpawner != null) boxSpawner.frequency = 0f;

        framesPerLoop = 60 * secondsPerLoop;
    }

    void NewMovement(FrameMovement frameMovement)
    {
        frameMovements.Add(frameMovement);
    }

    // Update is called once per frame
    void Update()
    {
        if(masterController == null) return;

        int frameOffset = Time.frameCount - startFrame;
        if (frameOffset % framesPerLoop == 0 && frameOffset > 0)
        {
            ClonePlayer();
            boxSpawner.Spawn();
        }

        for(int i = 0; i < slaveControllers.Count; i++)
        {
            int index = frameOffset - (i + 1) * framesPerLoop;
            if (index >= 0 && index < frameMovements.Count)
                slaveControllers[i].UpdateFromRecordedMovement(frameMovements[index]);
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

    void OnTriggerEnter(Collider other)
    {
        if(masterController == null && other.tag == "Player")
        {
            masterController = other.GetComponent<PlayerController>();
            masterController.movementEvent += NewMovement;

            startPosition = other.transform.position;

            slaveControllers = new List<ArtificialController>();
            frameMovements = new List<FrameMovement>();

            startFrame = Time.frameCount;
        }
    }
}
