using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCamera : MonoBehaviour
{
    public new Camera camera;
    public Transform target;
    public float distance = 4.0f;
    public Vector3 viewAngles;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (camera == null || target == null)
            return;

        Vector3 targetPosition = target.transform.position + Quaternion.Euler(viewAngles) * Vector3.forward * distance * 2.0f;
        transform.position = targetPosition;
        transform.forward = Quaternion.Euler(viewAngles) * -Vector3.forward;
        camera.orthographicSize = distance;
    }
}
