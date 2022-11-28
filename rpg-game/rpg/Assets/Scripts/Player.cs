using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Couples the client to the game
/// </summary>
public class Player : MonoBehaviour
{
    public static Player Current => current;
    protected static Player current;
    public Camera Camera => cameraController.camera;
    public CameraControllers.CameraController cameraController;
    public Input.Input input;
}
