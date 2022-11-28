using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace RPG
{
    /// <summary>
    /// Manages Game State
    /// </summary>
    public class Director : MonoBehaviour
    {
        public static Director Current => instance;
        static Director instance;
        public Battle.BattleManager battleManager;
        public Action<Party> onPartyMove;
        private void Awake()
        {
            if (instance == null)
                instance = this;
        }
        private void Update()
        {
            foreach (KeyValuePair<string, Party> pair in Party.parties)
            {
                pair.Value.Update();
            }

            battleManager?.Update();
        }
        private void OnDrawGizmos()
        {
            if (Party.parties != null)
            {
                foreach (Party party in Party.parties.Values)
                {
                    Gizmos.color = Color.yellow;
                    UnityEngine.BoundsInt bounds = party.GetBounds();
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                }
            }
        }
        void BeginBattle(params Party[] parties)
        {
            battleManager = Battle.BattleManager.New(parties);
        }
        public Party SpawnParty(Vector3Int coords, bool enemy = true)
        {
            string name = enemy ? $"Party {Party.parties.Count}" : "PLAYER";
            GameObject prefab = enemy ? Resources.Load<GameObject>("Prefabs/ENEMY") : Resources.Load<GameObject>("Prefabs/GAY CUBE PAWN");
            for (int i = 0; i < 5; i++)
            {
                GameObject pawnObject = Instantiate(prefab, coords, Quaternion.identity);
                Pawn pawn = pawnObject.GetComponent<Pawn>();
                Party.AddToParty(name, pawn);
            }

            Party party = Party.GetParty(name);
            party.onMove += OnPartyMove;
            return party;
        }
        public void OnPartyMove(Party party)
        {
            onPartyMove?.Invoke(party);

            if (party.IsPlayer)
            {
                UnityEngine.Bounds myBounds = new UnityEngine.Bounds { center = party.GetBounds().center, size = party.GetBounds().size } ;
                foreach (Party otherParty in Party.parties.Values)
                {
                    if (otherParty != party)
                    {
                        // Check distance to player
                        UnityEngine.Bounds otherBounds = new UnityEngine.Bounds { center = otherParty.GetBounds().center, size = otherParty.GetBounds().size };
                        float sqrDistance = otherBounds.SqrDistance(myBounds.ClosestPoint(otherBounds.center));
                        if (sqrDistance <= Mathf.Pow(2, 2))
                        {
                            // Initiate battle
                            BeginBattle(otherParty, party);
                            break;
                        }
                    }
                }
            }
        }
    }
}

