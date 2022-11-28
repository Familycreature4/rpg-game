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
        Vector3 angles = new Vector3(45.0f, 30.0f);
        Vector3 auxillaryAngles;
        bool useAuxillaryAngles = false;
        RPGPlayer player;
        Vector3 origin;
        public RPGController(RPGPlayer player)
        {
            this.player = player;
            player.input.Subscribe(this);

            player.onPartyChange += OnPartyChanged;
        }
        public override void LateUpdate()
        {
            Vector3 targetOriginPosition = origin;
            Quaternion targetRotation = Quaternion.Euler(angles);

            if (player != null && player.Party != null)
            {
                if (Director.Current.battleManager != null)
                    targetOriginPosition = Director.Current.battleManager.ActiveParty.GetCenter();
                else
                    targetOriginPosition = player.Party.GetCenter();

                if (useAuxillaryAngles)
                    targetRotation = Quaternion.Euler(auxillaryAngles);
                else
                    targetRotation = Quaternion.Euler(angles.x, player.Party.FormationRotation, angles.z);
            }


            origin = Vector3.Lerp(origin, targetOriginPosition, 10.0f * Time.deltaTime);
            Quaternion newRotation = useAuxillaryAngles ? targetRotation : Quaternion.Slerp(camera.transform.rotation, targetRotation, 10.0f * Time.deltaTime);

            camera.transform.position = origin + (newRotation * -Vector3.forward * 10.0f);
            camera.transform.rotation = newRotation;
        }
        public override void ReceiveInput(Input.Input input)
        {
            Input.RPGInput rpgInput = input as Input.RPGInput;

            if (rpgInput.rightClick.Pressed)
            {
                // Begin auxillary angles
                auxillaryAngles = camera.transform.rotation.eulerAngles;
                useAuxillaryAngles = true;
            }

            if (rpgInput.rightClick.Value && useAuxillaryAngles)
            {
                auxillaryAngles.x += rpgInput.mouseY.Value * 6.0f;
                auxillaryAngles.x = Mathf.Clamp(auxillaryAngles.x, -90f, 90f);
                auxillaryAngles.y += rpgInput.mouseX.Value * 6.0f;
            }
        }
        void OnPartyChanged(Party oldParty, Party newParty)
        {
            if (oldParty != null)
                oldParty.onMove -= OnPartyMove;

            newParty.onMove += OnPartyMove;
        }
        void OnPartyMove(Party party)
        {
            //useAuxillaryAngles = false;
        }
    }
}
