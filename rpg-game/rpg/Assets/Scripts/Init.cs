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
        RPG.Editor.Serializer.LoadWorldJSON("C:/Users/Shane/AppData/LocalLow/Nudsoft/RPG/Worlds/dungeon6.world", world);
        RPG.Director director = new GameObject("Director").AddComponent<RPG.Director>();

        // Spawn player party
        foreach (PartySpawn spawn in GameObject.FindObjectsOfType<PartySpawn>())
        {
            if (spawn.isPlayer)
            {
                RPG.Party party = director.SpawnParty(Vector3Int.FloorToInt(spawn.transform.position), false);
                RPG.RPGPlayer player = new GameObject("Player").gameObject.AddComponent<RPG.RPGPlayer>();
                player.SetParty(party);

                UIManager manager = new GameObject("UI Manager").AddComponent<UIManager>();
            }
            else
            {
                director.SpawnParty(Vector3Int.FloorToInt(spawn.transform.position), true);
            }
        }
    }
}
