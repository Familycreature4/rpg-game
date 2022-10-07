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
    float distance = 4.0f;
    public float targetDistance = 7.4f;
    float maxDistance = 7.4f;
    float minDistance = 1.0f;
    Vector3 viewAngles;
    public Vector3 targetViewAngles;
    Vector3 auxillaryViewAngles;  // Free look
    Vector3 origin;  // Point the camera rotates around
    public bool collide = false;

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

        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * 20.0f);

        Vector3 lookAt;
        if (Director.Current.activeBattle != null)
        {
            lookAt = Director.Current.activeBattle.ActiveParty.party.GetCenter();
        }
        else
        {
            lookAt = Client.Current.party.Leader.TileTransform.coordinates + Vector3.one / 2.0f + Vector3.up;
        }

        origin = Vector3.Lerp(origin, lookAt, Time.deltaTime * 20.0f);  // Smoothly adjust origin of the camera's orbit

        if (UnityEngine.Input.GetMouseButton(1))  // Rotate camera with free look
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                StartFreeLook();
            }

            auxillaryViewAngles.x = Mathf.Clamp(auxillaryViewAngles.x + UnityEngine.Input.GetAxisRaw("Mouse Y") * 6.0f, 0, 60.0f);
            auxillaryViewAngles.y += UnityEngine.Input.GetAxisRaw("Mouse X") * 6.0f;

            viewAngles = auxillaryViewAngles;
        }
        else  // Rotate camera with party direction
        {
            targetViewAngles.y = Client.Current.party.FormationRotation;
            // As the camera zooms in closer, set X angle to 5
            targetViewAngles.x = Utilities.MapRange(distance, minDistance, maxDistance, 5.0f, 45.0f);
            viewAngles = new Vector3(
                Mathf.LerpAngle(viewAngles.x, targetViewAngles.x, Time.deltaTime * 5.0f),
                Mathf.LerpAngle(viewAngles.y, targetViewAngles.y, Time.deltaTime * 5.0f),
                Mathf.LerpAngle(viewAngles.z, targetViewAngles.z, Time.deltaTime * 5.0f)
                );   
        }
        
        Vector3 cameraPosition = origin + Quaternion.Euler( viewAngles ) * -Vector3.forward * distance * 2.0f;

        //if (collide)
        //{
        //    Vector3 direction = (targetPosition - lookAt).normalized;
        //    float length = (targetPosition - lookAt).magnitude;
        //    RaycastHit[] hits = Physics.RaycastAll(lookAt, direction, length, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        //    System.Array.Sort(hits, delegate (RaycastHit a, RaycastHit b) { return a.distance.CompareTo(b.distance); });
        //    foreach (RaycastHit hit in hits)
        //    {
        //        if (hit.collider.gameObject.GetComponent<World>())
        //        {
        //            float dot = Vector3.Dot(direction, hit.normal);
        //            targetPosition = hit.point + (hit.normal * 0.125f) * (1.0f - Mathf.Abs(dot));
        //            break;
        //        }
        //    }
        //}

        transform.position = cameraPosition;
        transform.rotation = Quaternion.Euler(viewAngles);
        camera.orthographicSize = distance;
    }

    void StartFreeLook()
    {
        auxillaryViewAngles = viewAngles;
    }
}
