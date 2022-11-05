using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CameraControllers
{
    /// <summary>
    /// Freely floats around space - First person
    /// </summary>
    public class FreeController : CameraController, Input.IInputReceiver
    {
        public FreeController() : base()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }

        Vector3 velocity;
        Vector3 eyeAngles;
        float moveSpeed = 60.0f;
        float lookSpeed = 6.0f;
        public override void LateUpdate()
        {
            camera.transform.rotation = Quaternion.Euler(eyeAngles);
            camera.transform.position += velocity * Time.deltaTime;

            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 10.0f);
        }
        public void OnInputReceived(Input input)
        {
            eyeAngles.x += -input.mouseY.Value * lookSpeed;
            eyeAngles.y += input.mouseX.Value * lookSpeed;

            eyeAngles.x = Mathf.Clamp(eyeAngles.x, -90.0f, 90.0f);

            Vector3 move = Vector3.zero;

            if (input.confirm.Value)
                move.y += 1;

            if (input.left.Value)
                move.x -= 1;
            if (input.right.Value)
                move.x += 1;

            if (input.forward.Value)
                move.z += 1;
            if (input.backward.Value)
                move.z -= 1;

            Vector3 accel = camera.transform.rotation * move.normalized * moveSpeed;

            velocity += accel * Time.deltaTime;
        }
    }
}
