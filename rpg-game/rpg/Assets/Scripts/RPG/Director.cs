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

            OnBattleStart?.Invoke(activeBattle);

            //// Set ui stuff
            //foreach (UI.Party partyCanvas in GameObject.FindObjectsOfType<UI.Party>())
            //{
            //    GameObject.Destroy(partyCanvas.gameObject);
            //}

            //foreach (Party party in parties)
            //{
            //    UI.Party partyCanvas = GameObject.Instantiate(Resources.Load<GameObject>("UI/Party Canvas Prefab")).GetComponent<UI.Party>();
            //    partyCanvas.SetParty(party);
            //}
        }

        public void EndBattle()
        {
            state = State.Peace;
            activeBattle.OnPartyAttack -= OnBattlePartyAttack;
            activeBattle.OnPartyTurn -= OnBattlePartyTurn;
        }
    }
}

