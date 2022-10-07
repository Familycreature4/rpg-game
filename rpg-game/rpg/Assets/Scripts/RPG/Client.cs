using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG;
/// <summary>
/// Couples the user to the game
/// </summary>
public class Client : MonoBehaviour
{
    public static Client Current => instance;
    static Client instance;
    public Party party;
    public Input input = new Input();
    BoundsInt lastCameraBounds;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("DUPLICATE CLIENT");
        }
        else
        {
            instance = this;
        }
    }
    private void Update()
    {
        input.Update();

        #region PAWNS/PARTY
        if (party == null)
        {
            party = Party.GetParty("0");
        }

        if (party == null)
            return;

        bool didTurn = false;

        if (party.CanMove)
        {
            if (input.turnLeft.Value)
            {
                party.FormationRotation -= 90.0f;
                //input.turnLeft.Consume();
                didTurn = true;
            }
            if (input.turnRight.Value)
            {
                party.FormationRotation += 90.0f;
                //input.turnRight.Consume();
                didTurn = true;
            }
        }

        int xMove = 0;
        int zMove = 0;
        
        if (input.forward.Value)
            zMove++;
        if (input.backward.Value)
            zMove--;
        if (input.left.Value)
            xMove--;
        if (input.right.Value)
            xMove++;

        Vector3Int formationMove = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.FormationRotation, Vector3.up) * new Vector3(xMove, 0, zMove));
        party.Move(formationMove);

        if (didTurn)
        {
            party.Leader.InvokeMoveDelay();
        }
        #endregion

        #region CAMERA
        IsoCamera.Current.targetDistance -= UnityEngine.Input.GetAxisRaw("Mouse ScrollWheel") * 4.0f;
        if (UnityEngine.Input.GetMouseButton(1))
        {
            //IsoCamera.Current.viewAngles.y += Input.GetAxisRaw("Mouse X") * 15.0f;
            //IsoCamera.Current.viewAngles.x -= Input.GetAxisRaw("Mouse Y") * 15.0f;
            //IsoCamera.Current.viewAngles.x = Mathf.Clamp(IsoCamera.Current.viewAngles.x, -60, 1);
        }
        #endregion

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && Director.Current.activeBattle == null)
        {
            Director.Current.InitiateBattle(Party.GetParty("0"), Party.GetParty("1"));
        }
    }
}
