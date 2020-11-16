using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    public Behaviour[] componentsToDisable;
    Camera mainCam;

    bool active = false;
    float speed;
    public float defaultSpeed;
    public float speedUp;

    float defaultFOV;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = GetComponent<Camera>();
        defaultFOV = mainCam.fieldOfView;

        speed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            Physics.autoSimulation = active;
            foreach (var component in componentsToDisable)
                component.enabled = active;
            active = !active;

            if (!active)
            {
                mainCam.fieldOfView = defaultFOV;
            }
        }

        if(active)
        {
            float xLook = Input.GetAxis("Mouse X");
            float yLook = Input.GetAxis("Mouse Y");

            if(xLook != 0 || yLook != 0)
            {
                Vector3 eulerAngles = transform.rotation.eulerAngles;

                transform.rotation = Quaternion.Euler(eulerAngles.x - yLook, eulerAngles.y + xLook, eulerAngles.z);
            }

            float hMov = Input.GetAxisRaw("Horizontal");
            float vMov = Input.GetAxisRaw("Vertical");
            float lMov = 0;
            if (Input.GetKey(KeyCode.Q)) --lMov;
            if (Input.GetKey(KeyCode.E)) ++lMov;

            if (hMov != 0 || vMov != 0 || lMov != 0)
            {
                Vector3 move = transform.forward * vMov + transform.right * hMov + transform.up * lMov;
                move = move.normalized * Time.deltaTime * speed;
                transform.position += move;

                speed *= speedUp;
            }
            else
            {
                speed = defaultSpeed;
            }

            if (Input.GetKey(KeyCode.LeftBracket))
            {
                mainCam.fieldOfView -= 10 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightBracket))
            {
                mainCam.fieldOfView += 10 * Time.deltaTime;
            }
        }
    }
}
