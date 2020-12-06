using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWorkerOnEnter : MonoBehaviour
{
    [SerializeField] GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != player)
            other.transform.Find("Worker")?.gameObject.SetActive(false);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != player)
            other.transform.Find("Worker")?.gameObject.SetActive(true);
    }
}
