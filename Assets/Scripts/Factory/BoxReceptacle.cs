﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoxReceptacle : MonoBehaviour
{
    public Action<int> boxReceived = delegate { };

    public LayerMask boxLayer;
    private int boxLayerInt;

    int numBoxesReceived = 0;

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
            Destroy(other.gameObject);
        }
    }
}