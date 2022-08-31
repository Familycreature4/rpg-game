using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;
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

        Vector3 lookAt = target.transform.position;
        if (Director.Current.activeBattle != null)
        {
            lookAt = Director.Current.activeBattle.ActivePawn.TileTransform.coordinates;
        }
        else
        {
            lookAt = Client.Current.party.GetCenter();
        }
        Vector3 targetPosition = lookAt + Quaternion.Euler(viewAngles) * Vector3.forward * distance * 2.0f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 20.0f);
        transform.forward = Quaternion.Euler(viewAngles) * -Vector3.forward;
        camera.orthographicSize = distance;
    }
}
