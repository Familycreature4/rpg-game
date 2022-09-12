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

        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 lookAt = target.transform.position;
        if (Director.Current.activeBattle != null)
        {
            lookAt = Director.Current.activeBattle.ActiveParty.party.GetCenter();
        }
        else
        {
            lookAt = Client.Current.party.Leader.TileTransform.coordinates + Vector3.one / 2.0f;
        }

        origin = Vector3.Lerp(origin, lookAt, Time.deltaTime * 20.0f);

        Vector3 targetPosition = origin + Quaternion.Euler(viewAngles) * Vector3.forward * distance * 2.0f;

        if (collide)
        {
            Vector3 direction = (targetPosition - lookAt).normalized;
            float length = (targetPosition - lookAt).magnitude;
            RaycastHit[] hits = Physics.RaycastAll(lookAt, direction, length, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, delegate (RaycastHit a, RaycastHit b) { return a.distance.CompareTo(b.distance); });
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<World>())
                {
                    float dot = Vector3.Dot(direction, hit.normal);
                    targetPosition = hit.point + (hit.normal * 0.125f) * (1.0f - Mathf.Abs(dot));
                    break;
                }
            }
        }

        transform.position = targetPosition;
        transform.forward = Quaternion.Euler(viewAngles) * -Vector3.forward;
        camera.orthographicSize = distance;

        //// Alter fov based on distance to target
        //float distanceToTarget = Vector3.Distance(transform.position, lookAt);
        //float normValue = Mathf.Clamp01((distanceToTarget - maxFovDistance) / Mathf.Abs(maxFovDistance - minFovDistance));
        //float targetFov = normValue * (minFov - maxFov) + maxFov;
        //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFov, Time.deltaTime * 10.0f);
    }
}
