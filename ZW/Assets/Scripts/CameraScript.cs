using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    private float minFov = 30f;
    private float maxFov = 120f;
    private float sensitivity = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fov = Camera.main.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity * -1;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }

    // FixedUpdate is Update for physics
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * 15f * Time.fixedDeltaTime, Space.World);
        }

        else if (Input.mousePosition.y >= Screen.height * 0.98) { transform.Translate(Vector3.forward * 15f * Time.fixedDeltaTime, Space.World); }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.forward * 15f * -Time.fixedDeltaTime, Space.World);
        }

        else if (Input.mousePosition.y <= Screen.height * 0.02) { transform.Translate(Vector3.forward * 15f * -Time.fixedDeltaTime, Space.World); }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * 15f * Time.fixedDeltaTime, Space.World);
        }

        else if (Input.mousePosition.x >= Screen.width * 0.98) { transform.Translate(Vector3.right * 15f * Time.fixedDeltaTime, Space.World); }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.right * 15f * -Time.fixedDeltaTime, Space.World);
        }

        else if (Input.mousePosition.x <= Screen.width * 0.02) { transform.Translate(Vector3.right * 15f * -Time.fixedDeltaTime, Space.World); }  
    }
}
