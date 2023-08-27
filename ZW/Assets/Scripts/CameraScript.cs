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
        transform.Translate(Vector3.forward * 10f * Time.fixedDeltaTime * Input.GetAxis("Vertical"), Space.World);
        transform.Translate(Vector3.right * 10f * Time.fixedDeltaTime * Input.GetAxis("Horizontal"), Space.World);
    }
}
