using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        private void Awake()
        {
            if (instance == null)
                instance = this;
        }
        private void Update()
        {
            // Temporary driving code to initiate a battle
            if (Party.GetParty("0") != null && Party.GetParty("1") != null && activeBattle == null)
            {
                InitiateBattle(Party.GetParty("0"), Party.GetParty("1"));
            }

            // 
            if (state == State.Combat)
            {
                
            }

            activeBattle?.Update();
        }
        public void InitiateBattle(params Party[] parties)
        {
            Debug.Log("Initiating Battle");
            activeBattle = new Battle(parties);
            state = State.Combat;

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
    }
}

