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
        RPGPlayer player;
        Vector3 origin;
        public RPGController(RPGPlayer player)
        {
            this.player = player;
            player.input.Subscribe(this);
        }
        public override void LateUpdate()
        {
            Vector3 targetOriginPosition = origin;
            Quaternion targetRotation = Quaternion.Euler(angles);
            if (player != null && player.Party != null)
            {
                targetOriginPosition = player.Party.GetCenter();
                targetRotation = Quaternion.Euler(angles.x, player.Party.FormationRotation, angles.z);
            }

            origin = Vector3.Lerp(origin, targetOriginPosition, 10.0f * Time.deltaTime);
            Quaternion newRotation = Quaternion.Slerp(Quaternion.Euler(angles), targetRotation, 10.0f * Time.deltaTime);

            angles = newRotation.eulerAngles;

            camera.transform.position = origin + (newRotation * -Vector3.forward * 10.0f);
            camera.transform.rotation = newRotation;
        }
        public override void ReceiveInput(Input.Input input)
        {
            
        }
    }
}
