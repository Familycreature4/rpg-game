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
            // 
            if (state == State.Combat)
            {
                
            }
        }
        public void InitiateBattle(params Party[] parties)
        {
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

