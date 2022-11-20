using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Couples the client to the game
/// </summary>
public class Player : MonoBehaviour
{
    public Camera Camera => cameraController.camera;
    public CameraControllers.CameraController cameraController;
    public Input.Input input;
}
