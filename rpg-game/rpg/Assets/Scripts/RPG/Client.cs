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

        if (Input.GetKeyDown(KeyCode.Q))
            party.FormationRotation -= 90.0f;

        if (Input.GetKeyDown(KeyCode.E))
            party.FormationRotation += 90.0f;

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

        Vector3Int formationMove = Vector3Int.RoundToInt(Quaternion.AngleAxis(party.FormationRotation, Vector3.up) * new Vector3(xMove, 0, zMove));
        party.Move(formationMove);
        #endregion

        #region CAMERA
        IsoCamera.Current.distance -= Input.GetAxisRaw("Mouse ScrollWheel") * 4.0f;
        if (Input.GetMouseButton(1))
        {
            IsoCamera.Current.viewAngles.y += Input.GetAxisRaw("Mouse X") * 15.0f;
            IsoCamera.Current.viewAngles.x -= Input.GetAxisRaw("Mouse Y") * 15.0f;
            IsoCamera.Current.viewAngles.x = Mathf.Clamp(IsoCamera.Current.viewAngles.x, -60, 1);
        }
        #endregion

        if (Input.GetKeyDown(KeyCode.Space) && Director.Current.activeBattle == null)
        {
            Director.Current.InitiateBattle(Party.GetParty("0"), Party.GetParty("1"));
        }

        //UnityEngine.Bounds bounds = new UnityEngine.Bounds(party.GetCenter(), new Vector3(1, 1, 1));
        //bounds.Encapsulate(IsoCamera.Current.transform.position + Vector3.up * 4.0f);
        //BoundsInt intBounds = new BoundsInt((int)bounds.min.x, (int)bounds.min.y, (int)bounds.min.z, (int)bounds.size.x, (int)bounds.size.y, (int)bounds.size.z);
        //if (intBounds.min != lastCameraBounds.min || intBounds.max != lastCameraBounds.max)
        //{
        //    lastCameraBounds = intBounds;
        //    List<BoundsInt> listBounds = new List<BoundsInt>();
        //    listBounds.Add(intBounds);
        //    MeshGenerator.Generate(World.Current, listBounds);
        //}

        //DebugDraw.DrawCube(intBounds.center, intBounds.size, Color.white);
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
    void OnObjectSelected(GameObject ob)
    {
        if (ob.TryGetComponent<Pawn>(out Pawn pawn))
        {
            Debug.Log(pawn.name);
        }
    }
}
