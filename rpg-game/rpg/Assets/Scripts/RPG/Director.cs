using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Battle;
namespace RPG
{
    /// <summary>
    /// Manages Game State
    /// </summary>
    public class Director : MonoBehaviour
    {
        public static Director Current => instance;
        static Director instance;
        public Battle.Battle Battle => battleManager == null ? null : battleManager.currentBattle;
        public Battle.BattleManager battleManager;
        public List<Party> parties = new List<Party>();
        private void Awake()
        {
            if (instance == null)
                instance = this;

            battleManager = new BattleManager();

            EventManager.onPartyMove += OnPartyMove;
        }
        private void Update()
        {
            foreach (Party party in parties)
            {
                party.Update();
            }

            battleManager?.Update();
        }
        private void OnDrawGizmos()
        {
            if (parties != null)
            {
                foreach (Party party in parties)
                {
                    Gizmos.color = Color.yellow;
                    UnityEngine.BoundsInt bounds = party.GetBounds();
                    Gizmos.DrawWireCube(bounds.center, bounds.size);

                    UnityEditor.Handles.Label(bounds.center, party.name);
                }
            }
        }
        void BeginBattle(params Party[] parties)
        {
            battleManager.BeginBattle(parties);
        }
        public void AddParty(Party party)
        {
            parties.Add(party);
        }
        public void OnPartyMove(Party party)
        {
            if (Battle == null && party.IsPlayer)
            {
                UnityEngine.Bounds myBounds = new UnityEngine.Bounds { center = party.GetBounds().center, size = party.GetBounds().size } ;
                foreach (Party otherParty in parties)
                {
                    if (otherParty != party && otherParty.AllDead == false)
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

