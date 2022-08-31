using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// Battle state
    /// </summary>
    public class Battle
    {
        public Battle(params Party[] parties)
        {
            foreach (Party p in parties)
            {
                Debug.Log($"{p.pawns.Count}");
            }
            this.parties = parties;
        }
        public Party ActiveParty => parties[activePartyIndex];
        public Pawn ActivePawn => ActiveParty.pawns[activePawnIndex];
        /// <summary>
        /// List of parties involved
        /// </summary>
        Party[] parties;
        int activePartyIndex;
        /// <summary>
        /// Whose turn of the active party is it
        /// </summary>
        int activePawnIndex;

        public void NextTurn()
        {
            // Try incrementing the active pawn index
            // If max reached, increment active party index
            // Reset active pawn index

            activePawnIndex++;
            if (activePawnIndex >= ActiveParty.pawns.Count)
            {
                // All pawns had their turn
                // Give control to next party

                activePartyIndex++;
                if (activePartyIndex >= parties.Length)
                    activePartyIndex = 0;

                activePawnIndex = 0;
            }
        }
    }
}
