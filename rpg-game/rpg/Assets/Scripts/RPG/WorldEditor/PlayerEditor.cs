using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Editor.Selections;
namespace RPG.Editor
{
    public class PlayerEditor : Player
    {
        CameraControllers.FreeController CameraController
        {
            get
            {
                return cameraController as CameraControllers.FreeController;
            }
            set
            {
                cameraController = value;
            }
        }
        Selection currentSelection;
        private void Awake()
        {
            input = new Input();
            CameraController = new CameraControllers.FreeController();

            input.Subscribe(CameraController);

            currentSelection = new Sphere { radius = 10, center = new Vector3Int(10, 5, 30)};
        }
        public void Update()
        {
            input.Update();
            cameraController?.Update();

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                foreach (Vector3Int c in currentSelection.GetCoords)
                {
                    World.Current.SetTile(c, new Tile("Eye"));
                }
            }
        }

        public void LateUpdate()
        {
            cameraController?.LateUpdate();
        }
    }
}
