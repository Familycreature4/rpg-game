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
        public enum State
        {
            // No turns, unrestricted movement
            Peace,

            // Start counting turns, restrict movement
            Combat,  
        }
        public static Director Current => instance;
        static Director instance;
        public State state = State.Peace;
        public Battle activeBattle;
        public Action<Battle> OnBattleStart;
        public Action<Battle> OnBattleEnd;
        public Action<Battle.BattleParty> OnBattlePartyTurn;
        public Action<Battle.BattleParty> OnBattlePartyAttack;
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

            activeBattle?.Update();
        }
        public void InitiateBattle(params Party[] parties)
        {
            Debug.Log("Initiating Battle");
            activeBattle = new Battle(parties);
            state = State.Combat;

            // Subscribe the director events to the battle
            activeBattle.OnPartyAttack += OnBattlePartyAttack;
            activeBattle.OnPartyTurn += OnBattlePartyTurn;
            activeBattle.onBattleFinished += EndBattle;

            OnBattleStart?.Invoke(activeBattle);
        }
        public void EndBattle(Battle battle)
        {
            Debug.Log("DIRECTOR END BATTLE");
            foreach (Battle.BattleParty party in battle.parties)
            {
                foreach (Pawn pawn in party.party.pawns)
                {
                    pawn.ResetState();
                }
            }
            state = State.Peace;
            activeBattle = null;
            OnBattleEnd?.Invoke(battle);
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

            return Party.GetParty(name);
        }
        public void OnPartyMove(Party party)
        {
            onPartyMove?.Invoke(party);

            if (party.IsClient && activeBattle == null)
            {
                foreach (Party otherParty in Party.parties.Values)
                {
                    if (otherParty != Client.Current.party)
                    {
                        // Check distance to player
                        float distance = Vector3.Distance(otherParty.GetCenter(), party.GetCenter());
                        if (distance <= 4)
                        {
                            // Initiate battle
                            InitiateBattle(otherParty, Client.Current.party);
                            break;
                        }
                    }
                }
            }
        }
    }
}

