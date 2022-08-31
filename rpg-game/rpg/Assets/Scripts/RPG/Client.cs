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
        if (party == null)
        {
            party = Party.GetParty("0");
        }

        if (party == null)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            party.formationRotation -= 90.0f;

        if (Input.GetKeyDown(KeyCode.E))
            party.formationRotation += 90.0f;

        int xMove = 0;
        int zMove = 0;
        if (Input.GetKey(KeyCode.A))
            xMove--;
        if (Input.GetKey(KeyCode.D))
            xMove++;
        if (Input.GetKey(KeyCode.W))
            zMove++;
        if (Input.GetKey(KeyCode.S))
            zMove--;

        Vector3Int formationMove = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.formationRotation, Vector3.up) * new Vector3(xMove, 0, zMove));
        party.Move(formationMove);
        
    }

    private void OnDrawGizmos()
    {
        if (party == null)
            return;

        foreach (Vector3Int local in party.formationLocalPositions)
        {
            Gizmos.DrawCube(World.WorldCoordToScene(party.TransformFormationPosition(local) + Vector3.one / 2.0f), new Vector3(1, 1, 1));
        }
    }
}
