using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantTextureSize : MonoBehaviour
{
    public float timesToTileX;
    public float timesToTileY;

    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(transform.lossyScale.x / timesToTileX, transform.lossyScale.z / timesToTileY);
    }

}
