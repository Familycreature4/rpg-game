using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraControllers
{
    public class OrbitController : CameraController
    {
        public OrbitController()
        {
            Cursor.lockState = CursorLockMode.None;
            camera = Camera.main;
        }
        public Vector3 origin;
        public Vector3 angles;
        Vector3 localOffset;
        public float distance = 10.0f;
        float lookSpeed = 6.0f;
        float zoomSpeed = 10.0f;
        public override void LateUpdate()
        {
            camera.transform.position = origin + Quaternion.Euler(angles) * localOffset;
            camera.transform.rotation = Quaternion.Euler(angles);
        }
        void BeginOrbit()
        {
            Ray ray = camera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 worldPosition = origin + Quaternion.Euler(angles) * localOffset;
                origin = hit.point;
                
            }
        }
        public override void ReceiveInput(Input.Input input)
        {
            Input.RPGInput rpgInput = input as Input.RPGInput;

            bool middleMouse = rpgInput.middleClick.Value;
            bool shift = rpgInput.shift.Value;

            if (middleMouse && shift)  // Pan
            {
                localOffset.x -= rpgInput.mouseX.Value;
                localOffset.y -= rpgInput.mouseY.Value;
            }
            else if (middleMouse)  // Orbit
            {
                if (rpgInput.shift.Pressed)
                    BeginOrbit();

                angles.x -= rpgInput.mouseY.Value * lookSpeed;
                angles.y += rpgInput.mouseX.Value * lookSpeed;

                angles.x = Mathf.Clamp(angles.x, -90f, 90f);
            }

            localOffset.z += -rpgInput.mouseScroll.Value * zoomSpeed;
        }
    }
}