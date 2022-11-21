using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Editor.Entities;
public class Init : MonoBehaviour
{
    private void Start()
    {
        // Load world
        RPG.World world = new GameObject("World").AddComponent<RPG.World>();
        RPG.Editor.Serializer.LoadWorldJSON("C:/Users/Shane/AppData/LocalLow/Nudsoft/RPG/Worlds/dungeon4.world", world);

        // Spawn player party
        PartySpawn spawn = GameObject.FindObjectOfType<PartySpawn>();
        if (spawn != null)
        {
            // Create party
            GameObject pawnPrefab = Resources.Load<GameObject>("Prefabs/GAY CUBE PAWN");
            for (int i = 0; i < 5; i++)
            {
                RPG.Pawn pawn = GameObject.Instantiate<GameObject>(pawnPrefab, spawn.transform.position, Quaternion.identity, null).GetComponent<RPG.Pawn>();
                pawn.Coordinates = Vector3Int.FloorToInt(spawn.transform.position);

                RPG.Party.AddToParty("Player Party", pawn);
            }
        }
        else
        {
            Debug.Log("No spawn found");
        }
    }
}
