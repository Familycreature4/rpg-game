using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;
public class IsoCamera : MonoBehaviour, Input.IInputReceiver
{
    public static IsoCamera Current => instance;
    static IsoCamera instance;
    public new Camera camera;
    float distance = 4.0f;
    float targetDistance = 7.4f;
    float maxDistance = 4.0f;
    float minDistance = 1.0f;
    bool useAuxillaryAngles = false;
    Vector3 viewAngles;
    Vector3 targetViewAngles;
    Vector3 auxillaryViewAngles;  // Free look
    Vector3 origin;  // Point the camera rotates around
    float moveSpeed = 10.0f;
    bool collide = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        camera = GetComponent<Camera>();

        Client.Current.input.Subscribe(this);

        // Disable auxillary angles if the party moves
        Director.Current.onPartyMove += delegate (Party party)
        {
            if (party.IsClient)
            {
                useAuxillaryAngles = false;
            }
        };
    }
    private void LateUpdate()
    {
        if (camera == null)
            return;

        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        distance = Mathf.Lerp(distance, targetDistance, Time.deltaTime * 20.0f);

        Vector3 lookAt;
        if (Director.Current.activeBattle != null)
        {
            lookAt = Director.Current.activeBattle.ActiveParty.party.GetCenter();
            useAuxillaryAngles = true;
        }
        else
        {
            lookAt = Client.Current.party.Leader.TileTransform.coordinates + Vector3.one / 2.0f + Vector3.up;
        }

        origin = Vector3.Lerp(origin, lookAt, Time.deltaTime * moveSpeed);  // Smoothly adjust origin of the camera's orbit

        if (useAuxillaryAngles)  // Rotate camera with free look
        {
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

        if (collide)
        {
            Vector3 direction = (cameraPosition - origin).normalized;
            float length = (cameraPosition - origin).magnitude;
            RaycastHit[] hits = Physics.SphereCastAll(origin, 0.1f, direction, length, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, delegate (RaycastHit a, RaycastHit b) { return a.distance.CompareTo(b.distance); });
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<RPG.ChunkComponent>())
                {
                    float dot = Vector3.Dot(direction, hit.normal);
                    cameraPosition = origin + direction * hit.distance;
                    //cameraPosition = hit.point + (hit.normal * 0.125f) * (1.0f - Mathf.Abs(dot));
                    break;
                }
            }
        }

        transform.position = cameraPosition;
        transform.rotation = Quaternion.Euler(viewAngles);
        camera.orthographicSize = distance;
    }
    public void SetLookTarget(Vector3 target)
    {
        origin = target;
    }
    void StartFreeLook()
    {
        auxillaryViewAngles = viewAngles;
    }
    public void OnInputReceived(Input input)
    {
        if (input.rightClick.Pressed)
        {
            StartFreeLook();
        }

        if (input.rightClick.Value)
        {
            auxillaryViewAngles.x = Mathf.Clamp(auxillaryViewAngles.x + input.mouseY.Value * 6.0f, 0, 60.0f);
            auxillaryViewAngles.y += input.mouseX.Value * 6.0f;
            useAuxillaryAngles = true;
        }

        if (input.rightClick.Pressed)
            input.rightClick.Consume();

        this.targetDistance -= input.mouseScroll.Value * 4.0f;
    }
    public int GetInputPriority() => 100;
}
