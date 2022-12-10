using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Entities;
public class Init : MonoBehaviour
{
    public string mapName;
    private void Start()
    {
        // Load world
        RPG.World world = new GameObject("World").AddComponent<RPG.World>();
        RPG.Editor.Serializer.LoadWorldJSON($"{RPG.Editor.Serializer.DirectoryPath}/{mapName}.world", world);
        RPG.Director director = new GameObject("Director").AddComponent<RPG.Director>();

        // Spawn player party
        foreach (PartySpawn spawn in GameObject.FindObjectsOfType<PartySpawn>())
        {
            Vector3Int coords = Vector3Int.FloorToInt(spawn.transform.position);
            if (spawn.isPlayer)
            {
                RPG.Party party = RPG.Party.FromJson($"{Application.persistentDataPath}/data/party.json", coords);
                if (party == null)
                    party = RPG.Party.Spawn(coords, true);

                RPG.RPGPlayer player = new GameObject("Player").gameObject.AddComponent<RPG.RPGPlayer>();
                player.SetParty(party);

                director.AddParty(party);

                RPG.UI.UIManager manager = GameObject.Find("UI").AddComponent<RPG.UI.UIManager>();
            }
            else
            {
                director.AddParty(RPG.Party.Spawn(coords, false));
            }
        }
    }
}
