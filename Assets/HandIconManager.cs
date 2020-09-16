using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIconManager : MonoBehaviour
{

    public Texture open;
    public Texture close;

    UnityEngine.UI.RawImage img;

    void Start()
    {
        img = GetComponent<UnityEngine.UI.RawImage>();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
        img.texture = open;
    }

    public void Close()
    {
        gameObject.SetActive(true);
        img.texture = close;
    }
}
