using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] airWhooshes;
    [SerializeField] AudioClip[] impacts;

    [SerializeField] float airWhooshSpeed = 2f;

    Rigidbody _rb;

    float lastSpeed = 0;
    float SpeedDiff => Mathf.Abs(lastSpeed - _rb.velocity.sqrMagnitude);

    bool whooshed = false;

    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_rb.velocity.sqrMagnitude > airWhooshSpeed * airWhooshSpeed && !whooshed)
        {
            AudioHelper.PlayRandomClipFromArray(airWhooshes, 0.5f, 1f, transform);
            whooshed = true;
        }
        else if(_rb.velocity.sqrMagnitude < airWhooshSpeed * airWhooshSpeed && whooshed)
        {
            whooshed = false;
        }

        lastSpeed = _rb.velocity.sqrMagnitude;
    }

    void OnCollisionEnter(Collision other)
    {
        AudioHelper.PlayRandomClipFromArray(impacts, SpeedDiff / 2f, 1f, transform);
    }
}
