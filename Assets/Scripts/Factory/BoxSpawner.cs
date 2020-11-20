using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoxSpawner : MonoBehaviour
{
    public GameObject box;
    List<GameObject> spawnedBoxes;

    public float frequency;
    private float timer;

    [SerializeField] GameObject destroyParticles;

    // Use this for initialization
    void Start()
    {
        timer = 0f;
        spawnedBoxes = new List<GameObject>();
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
        var newBox = Instantiate(box, transform.position, transform.rotation);
        spawnedBoxes.Add(newBox);

        while (spawnedBoxes[0] == null) spawnedBoxes.RemoveAt(0);
    }

    public void DestroyAll()
    {
        if (spawnedBoxes == null || spawnedBoxes.Count == 0) return;

        while (spawnedBoxes[0] == null) spawnedBoxes.RemoveAt(0);
        while (spawnedBoxes.Count > 0)
        {
            Instantiate(destroyParticles, spawnedBoxes[0].transform.position, Quaternion.identity);
            Destroy(spawnedBoxes[0]);
            spawnedBoxes.RemoveAt(0);
        }
    }
}
