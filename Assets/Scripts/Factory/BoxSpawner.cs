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
        timer += Time.deltaTime;
        if (timer >= 1f / frequency)
        {
            Instantiate(box, this.transform.position, this.transform.rotation);
            timer = 0f;
        }
    }
}
