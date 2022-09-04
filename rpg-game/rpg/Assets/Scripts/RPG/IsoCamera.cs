using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;
public class IsoCamera : MonoBehaviour
{
    public static IsoCamera Current => instance;
    static IsoCamera instance;
    public new Camera camera;
    public Transform target;
    public float distance = 4.0f;
    float maxDistance = 7.4f;
    float minDistance = 1.0f;
    public Vector3 viewAngles;
    Vector3 origin;  // Point the camera rotates around

    private void Awake()
    {
        if (instance == null)
            instance = this;
        camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (camera == null || target == null)
            return;

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 lookAt = target.transform.position;
        if (Director.Current.activeBattle != null)
        {
            lookAt = Director.Current.activeBattle.ActivePawn.TileTransform.coordinates;
        }
        else
        {
            lookAt = Client.Current.party.GetCenter();
        }

        origin = Vector3.Lerp(origin, lookAt, Time.deltaTime * 20.0f);

        Vector3 targetPosition = origin + Quaternion.Euler(viewAngles) * Vector3.forward * distance * 2.0f;
        transform.position = targetPosition;
        transform.forward = Quaternion.Euler(viewAngles) * -Vector3.forward;
        camera.orthographicSize = distance;
    }
}
