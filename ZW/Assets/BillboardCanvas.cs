using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCanvas : MonoBehaviour
{
    Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + cameraTransform.rotation * -Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}
