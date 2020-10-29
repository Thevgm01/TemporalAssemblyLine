using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIconManager : MonoBehaviour
{
    public enum State { OPEN, CLOSED, HIDDEN };
    private State curState;

    public Camera _camera;

    public Texture openImage;
    public Texture closedImage;

    UnityEngine.UI.RawImage img;

    Transform grabbedTransform;
    Vector3 centerOfScreen;
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The amount that the open hand icon follows the object in real world space. " +
        "At 0 it stays in the center of the screen, while at 1 it perfectly follows the object.")]
    float openHandLag = 0.5f;
    [SerializeField]
    [Range(0f, 1f)]
    [Tooltip("The amount that the closed hand icon follows the object in real world space. " +
        "At 0 it stays in the center of the screen, while at 1 it perfectly follows the object.")]
    float closedHandLag = 0.5f;

    void Awake()
    {
        img = GetComponent<UnityEngine.UI.RawImage>();

        centerOfScreen = _camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));
    }

    void Update()
    {
        if (img.enabled && grabbedTransform != null)
        {
            img.transform.position =
                Vector3.Lerp(centerOfScreen, _camera.WorldToScreenPoint(grabbedTransform.position),
                curState == State.OPEN ? openHandLag : closedHandLag);
        }
    }

    public void Open(Transform t) { SetState(State.OPEN); grabbedTransform = t; }
    public void Close(Transform t) { SetState(State.CLOSED); grabbedTransform = t; }
    public void Hide() { SetState(State.HIDDEN); }

    void SetState(State newState)
    {
        if (curState == newState) return;
        else if(newState == State.OPEN)
        {
            img.texture = openImage;
            img.enabled = true;
        }
        else if(newState == State.CLOSED)
        {
            img.texture = closedImage;
            img.enabled = true;
        }
        else if(newState == State.HIDDEN)
        {
            img.enabled = false;
        }

        curState = newState;
    }
}
