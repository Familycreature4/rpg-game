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
        public Action<Battle> onBattleEnd;
        public Action<Battle.BattleParty> OnBattlePartyTurn;
        public Action<Battle.BattleParty> OnBattlePartyAttack;
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
            onBattleEnd?.Invoke(battle);
            state = State.Peace;
            activeBattle = null;
        }
    }
}

