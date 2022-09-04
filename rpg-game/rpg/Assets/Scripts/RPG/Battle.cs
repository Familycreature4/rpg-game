using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// Stores the state of a battle among parties
    /// </summary>
    public class Battle
    {
        public enum State
        {
            Planning,
            Attacking
        }
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
        Party[] parties;
        int activePartyIndex;
        int activePawnIndex;
        public State state;
        List<AttackInfo> attacks = new List<AttackInfo>();
        Pawn attackerPawnSelection = null;
        Pawn targetPawnSelection = null;
        float botThinkTime = 1.0f;
        float nextBotThinkTime = 0;
        public void Update()
        {
            foreach (AttackInfo attack in attacks)
            {
                Debug.DrawLine(attack.attacker.transform.position, attack.target.transform.position);
            }
            if (ActiveParty == Client.Current.party)
            {
                // Allow manual definition of attacks
                // Await for attack
                if (Input.GetMouseButtonDown(0))
                {
                    Pawn selectedPawn = Client.Current.GetSelection();
                    if (selectedPawn)
                    {
                        bool isEnemy = selectedPawn.party != ActiveParty;
                        if (isEnemy)
                            targetPawnSelection = selectedPawn;
                        else
                            attackerPawnSelection = selectedPawn;
                    }
                }

                if (targetPawnSelection != null && attackerPawnSelection != null)
                {
                    // Create attack info
                    // Check if an existing attackInfo exists for attacker
                    for (int i = 0; i < attacks.Count; i++)
                    {
                        AttackInfo attackInfo = attacks[i];
                        if (attackInfo.attacker == attackerPawnSelection)
                        {
                            // Update this
                            attacks[i] = new AttackInfo { attacker = attackerPawnSelection, target = targetPawnSelection };
                            targetPawnSelection = null;
                            attackerPawnSelection = null;
                            return;
                        }
                    }

                    // If control reaches here, create new attack info
                    attacks.Add(new AttackInfo { attacker = attackerPawnSelection, target = targetPawnSelection });
                    targetPawnSelection = null;
                    attackerPawnSelection = null;
                }

                if (CanExecute() && Input.GetKeyDown(KeyCode.Space))
                {
                    DoAttack();
                    NextTurn();
                }
            }
            else
            {
                // Define attacks for bot
                if (attacks.Count == 0)
                {
                    for (int i = 0; i < ActiveParty.pawns.Count; i++)
                    {
                        attacks.Add(new AttackInfo { attacker = ActiveParty.pawns[i], target = GetRandomEnemyPawn(ActiveParty) });
                    }

                    nextBotThinkTime = Time.time + botThinkTime;
                }

                if (Time.time >= nextBotThinkTime)
                {
                    DoAttack();
                    NextTurn();
                }
            }
        }
        Pawn GetRandomEnemyPawn(Party myParty)
        {
            foreach (Party p in parties)
            {
                if (p == myParty)
                    continue;

                return p.pawns[Random.Range(0, p.pawns.Count)];
            }

            return null;
        }
        /// <summary>
        /// Gives control to the next party
        /// </summary>
        void NextTurn()
        {
            activePartyIndex++;
            if (activePartyIndex >= parties.Length)
                activePartyIndex = 0;

            attacks.Clear();
            Debug.Log($"It is now {ActiveParty}'s turn");
        }
        /// <summary>
        /// Executes attacks
        /// </summary>
        void DoAttack()
        {
            foreach (AttackInfo attack in attacks)
            {
                attack.target.TakeDamage();
            }
        }
        bool CanExecute()
        {
            return attacks.Count == ActiveParty.pawns.Count;
        }
        /// <summary>
        /// Data describing which pawn will attack who with what
        /// </summary>
        public struct AttackInfo
        {
            public Pawn attacker;
            public Pawn target;
        }
    }
}
