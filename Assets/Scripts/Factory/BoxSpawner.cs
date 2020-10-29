using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject box;
    public float frequency;

    private float timer;

    // Use this for initialization
    void Start()
    {
        timer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (frequency <= 0) return;

        timer += Time.deltaTime;
        if (timer >= 1f / frequency)
        {
            Spawn();
            timer = 0f;
        }
    }

    public void Spawn()
    {
        Instantiate(box, this.transform.position, this.transform.rotation);
    }
}
