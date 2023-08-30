using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float fov;
    [SerializeField] private float minFov = 30f;
    [SerializeField] private float maxFov = 120f;
    [SerializeField] private float sensitivity = 20f;
    [SerializeField] private float velocity = 0F;
    [SerializeField] private float smoothTime = 0.1F;

    // Start is called before the first frame update
    void Start()
    {
        fov = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        fov -= scroll * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = Mathf.SmoothDamp(Camera.main.fieldOfView, fov, ref velocity, smoothTime) ;
    }

    // FixedUpdate is Update for physics
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * 15f * Time.fixedDeltaTime, Space.World);
        }
        // Mouse on the upper edge of the screen
        else if (Input.mousePosition.y >= Screen.height * 0.98) { transform.Translate(Vector3.forward * 15f * Time.fixedDeltaTime, Space.World); }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.forward * 15f * -Time.fixedDeltaTime, Space.World);
        }
        // Mouse on the lower edge of the screen
        else if (Input.mousePosition.y <= Screen.height * 0.02) { transform.Translate(Vector3.forward * 15f * -Time.fixedDeltaTime, Space.World); }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * 15f * Time.fixedDeltaTime, Space.World);
        }
        // Mouse on the right edge of the screen
        else if (Input.mousePosition.x >= Screen.width * 0.98) { transform.Translate(Vector3.right * 15f * Time.fixedDeltaTime, Space.World); }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.right * 15f * -Time.fixedDeltaTime, Space.World);
        }
        // Mouse on the left edge of the screen
        else if (Input.mousePosition.x <= Screen.width * 0.02) { transform.Translate(Vector3.right * 15f * -Time.fixedDeltaTime, Space.World); }  
    }
}
