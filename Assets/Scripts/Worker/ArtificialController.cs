using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtificialController : MonoBehaviour
{
    GrabController _grabber;
    MovementController _movement;
    Transform _head;
    Collider _body;
    FootCollider _feet;

    public AnimationCurve xInput;
    public AnimationCurve yInput;

    float timeTracker;

    void Awake()
    {
        _grabber = GetComponent<GrabController>();
        _movement = GetComponent<MovementController>();
        _body = GetComponent<Collider>();
        _head = transform.Find("Head");
        _feet = GetComponentInChildren<FootCollider>();

        timeTracker = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeTracker += Time.deltaTime;

        Vector3 newMove = new Vector3(xInput.Evaluate(timeTracker), 0f, yInput.Evaluate(timeTracker));
        _movement.forceNextFrame += newMove * Time.deltaTime;
    }
}
