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
        #region PAWNS/PARTY
        if (party == null)
        {
            party = Party.GetParty("0");
        }

        if (party == null)
            return;

        if (party.CanMove)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                party.FormationRotation -= 90.0f;
                party.Leader.InvokeMoveDelay();
            }
            if (Input.GetKey(KeyCode.E))
            {
                party.FormationRotation += 90.0f;
                party.Leader.InvokeMoveDelay();
            }
        }
        int xMove = 0;
        int zMove = 0;
        
        if (Input.GetKey(KeyCode.W))
            zMove++;
        if (Input.GetKey(KeyCode.S))
            zMove--;
        if (Input.GetKey(KeyCode.A))
            xMove--;
        if (Input.GetKey(KeyCode.D))
            xMove++;

        Vector3Int formationMove = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.FormationRotation, Vector3.up) * new Vector3(xMove, 0, zMove));
        party.Move(formationMove);
        #endregion

        #region CAMERA
        IsoCamera.Current.distance -= Input.GetAxisRaw("Mouse ScrollWheel") * 4.0f;
        if (Input.GetMouseButton(1))
        {
            //IsoCamera.Current.viewAngles.y += Input.GetAxisRaw("Mouse X") * 15.0f;
            //IsoCamera.Current.viewAngles.x -= Input.GetAxisRaw("Mouse Y") * 15.0f;
            //IsoCamera.Current.viewAngles.x = Mathf.Clamp(IsoCamera.Current.viewAngles.x, -60, 1);
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Space) && Director.Current.activeBattle == null)
        {
            Director.Current.InitiateBattle(Party.GetParty("0"), Party.GetParty("1"));
        }
    }
}
