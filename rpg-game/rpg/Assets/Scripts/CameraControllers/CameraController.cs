using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CameraControllers
{
    /// <summary>
    /// Manipulates the Player's Camera component
    /// </summary>
    public class CameraController
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
    }

}
