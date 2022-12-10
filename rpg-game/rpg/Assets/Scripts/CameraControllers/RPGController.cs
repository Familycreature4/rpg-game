using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG;
using UnityEngine;
namespace CameraControllers
{
    public class RPGController : CameraController
    {
        public RPGController(RPGPlayer player)
        {
            this.player = player;
            EventManager.OnInput += OnInput;
            EventManager.OnSelect += OnSelect;
            EventManager.onAttackerTurnStart += OnAttackerTurnStart;
        }
        RPGPlayer player;
        Vector3 origin;
        float distance;
        float maxDistance = 16.0f;
        Vector3 angles;
        Vector3 originOffset;
        float targetDistance = 6.0f;
        Vector3 targetAngles = new Vector3(40, 0, 0);
        Func<Vector3> originGetter;
        bool useLerp = true;
        bool useCollision = true;
        public override void LateUpdate()
        {
            useLerp = true;

            Vector3 targetOrigin = Vector3.zero;
            if (originGetter != null)
            {
                targetOrigin = originGetter();
            }
            else if (player != null && player.Party != null)
            {
                targetOrigin = player.Party.GetCenter();
            }
            // Update camera transform

            distance = useLerp ? Mathf.Lerp(distance, targetDistance, 10.0f * Time.deltaTime) : targetDistance;

            // Clamp offset distance
            float maxDistance = Mathf.Abs(this.maxDistance - targetDistance);
            originOffset = Vector3.ClampMagnitude(originOffset, maxDistance);

            origin = useLerp ? Vector3.Lerp(origin, targetOrigin + originOffset, 20.0f * Time.deltaTime) : targetOrigin + originOffset;
            angles = useLerp ? Quaternion.Slerp(Quaternion.Euler(angles), Quaternion.Euler(targetAngles), 20.0f * Time.deltaTime).eulerAngles : targetAngles;

            Vector3 cameraTargetPosition = origin + Quaternion.Euler(angles) * -Vector3.forward * distance;

            if (useCollision)
            {
                Vector3 origin = targetOrigin;
                Vector3 difference = cameraTargetPosition - origin;
                RaycastHit[] hits = Physics.SphereCastAll(origin, 0.2f, difference, difference.magnitude, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
                Array.Sort(hits, (RaycastHit a, RaycastHit b) => { return a.distance.CompareTo(b.distance); });

                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.gameObject.GetComponent<Pawn>() != null)
                    {
                        continue;
                    }

                    cameraTargetPosition = origin + hit.distance * difference.normalized;
                    break;
                }
            }

            camera.transform.position = cameraTargetPosition;
            camera.transform.rotation = Quaternion.Euler(angles);
        }
        void OnInput(Input.RPGInput input)
        {
            bool hideCursor = input.rightClick.Value || input.middleClick.Value;

            if (hideCursor)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                useLerp = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                useLerp = true;
            }

            if (input.rightClick.Value)
            {
                targetAngles.x += input.mouseY.Value * 6.0f;
                targetAngles.x = Mathf.Clamp(targetAngles.x, -90, 90);

                targetAngles.y += input.mouseX.Value * 6.0f;
            }
            else if (input.middleClick.Value)  // Pan
            {
                originOffset += Quaternion.Euler(targetAngles) * new Vector3(-input.mouseX.Value, -input.mouseY.Value, 0.0f) * 0.5f;
            }

            targetDistance = Mathf.Clamp(targetDistance + input.mouseScroll.Value * 6.0f, 2.0f, maxDistance);
            
        }
        void OnSelect(RPG.Events.SelectArgs args)
        {
            if (args.IsPawn)
            {
                originOffset = Vector3.zero;
                Pawn pawn = args.Pawn;
                this.originGetter = delegate () { return pawn.Coordinates + Vector3.one / 2.0f; };
            }
        }
        void OnAttackerTurnStart(RPG.Battle.Battle battle, RPG.Battle.Attackers.Attacker attacker)
        {
            originOffset = Vector3.zero;
            this.originGetter = delegate () { return attacker.Party.GetCenter(); };
        }
    }
}
