using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CameraControllers
{
    /// <summary>
    /// Manipulates the Player's Camera component
    /// </summary>
    public class CameraController : Input.IInputReceiver
    {
        public CameraController()
        {
            camera = Camera.main;
            if (camera == null)
                camera = GameObject.FindObjectOfType<Camera>();
        }
        public Camera camera;
        public virtual void Update()
        {

        }
        public virtual void LateUpdate()
        {

        }
        public virtual void ReceiveInput(Input.Input input)
        {

        }
        public void OnInputReceived(Input.Input input)
        {
            ReceiveInput(input);
        }

        public int GetInputPriority()
        {
            return 10000;
        }
    }

}
