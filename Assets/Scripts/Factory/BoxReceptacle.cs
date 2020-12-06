using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoxReceptacle : MonoBehaviour
{
    [SerializeField] AudioClip receivedSound;

    public Action<int> boxReceived = delegate { };

    public LayerMask boxLayer;
    private int boxLayerInt;

    int numBoxesReceived = 0;

    [SerializeField] GameObject boxReceivedParticles;

    void Awake()
    {
        boxLayerInt = (int)Mathf.Log(boxLayer.value, 2);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == boxLayerInt)
        {
            numBoxesReceived++;
            boxReceived?.Invoke(numBoxesReceived);
            Instantiate(boxReceivedParticles, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            AudioHelper.PlayClip(receivedSound, 1f, 1f, transform);
        }
    }
}
