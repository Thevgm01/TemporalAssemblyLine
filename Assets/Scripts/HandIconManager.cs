using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIconManager : MonoBehaviour
{
    public enum State { OPEN, CLOSED, HIDDEN };
    private State curState;

    public Texture openImage;
    public Texture closedImage;

    UnityEngine.UI.RawImage img;

    public void SetState(State newState)
    {
        if (img == null) img = GetComponent<UnityEngine.UI.RawImage>();
        if (curState == newState) return;
        else if(newState == State.OPEN)
        {
            img.texture = openImage;
            gameObject.SetActive(true);
        }
        else if(newState == State.CLOSED)
        {
            img.texture = closedImage;
            gameObject.SetActive(true);
        }
        else if(newState == State.HIDDEN)
        {
            gameObject.SetActive(false);
        }

        curState = newState;
    }
}
