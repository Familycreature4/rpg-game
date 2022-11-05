using System;
using System.Collections.Generic;
using RPG.Items;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// Stores and manages the state of a battle among parties
    /// </summary>
    /// 
    /*
    Gives control to each party as turns
    Party in control returns a list of attacks to do
    Battle then iterates through each attack
    Give control to next, valid party
     */
    public partial class Battle
    {
        public enum State
        {
            Waiting,
            Attacking
        }
        public Battle(params Party[] parties)
        {
            this.parties = new BattleParty[parties.Length];
            for (int i = 0; i < parties.Length; i++)
            {
                Party party = parties[i];
                BattleParty battleParty;
                if (party.IsClient)
                {
                    battleParty = new ClientBattle(party, this);
                    activePartyIndex = i;
                }
                else
                    battleParty = new BotBattle(parties[i], this);

                this.parties[i] = battleParty;
            }

            OnPartyTurn?.Invoke(ActiveParty);
        }
        public BattleParty ActiveParty => parties[activePartyIndex];
        public BattleParty[] parties;
        int activePartyIndex;
        int currentAttackIndex;
        public State state;
        public Action<BattleParty> OnPartyTurn;  // Invoked when the active party is active
        public Action<BattleParty> OnPartyAttack;  // Invoked when the active party begins attack / Ends planning
        public Action<Battle> onBattleFinished;
        List<AttackInfo> attacks;
        float nextPawnAttackTime = 0.0f;
        float nextPawnAttackDelay = 1.0f;
        bool doAttacks = false;
        bool firstAttacked = false;
        bool secondAttacked = false;
        public void Update()
        {
            ActiveParty?.TurnThink();
            ActiveParty?.DebugAttacks();

            AttackUpdate();
        }
        /// <summary>
        /// Iterates over the current attacks
        /// </summary>
        void AttackUpdate()
        {
            if (doAttacks)
            {
                if (currentAttackIndex >= attacks.Count)  // If all attacks have been processed for this party's turn
                {
                    // Check if a party is dead after attacks
                    doAttacks = false;

                    OnPartyAttack?.Invoke(ActiveParty);

                    // Check if this is the last party standing
                    bool allDead = true;
                    foreach (BattleParty party in parties)
                    {
                        if (party == ActiveParty)
                            continue;

                        if (party.AllDead == false)
                        {
                            allDead = false;
                            break;
                        }
                    }

                    if (allDead)
                    {
                        // ENd battle, this party WINS!!! >?:)
                        EndBattle();
                    }
                    else
                    {
                        NextTurn();
                    }

                }
                else if (nextPawnAttackTime <= Time.time)
                {
                    AttackInfo attack = attacks[currentAttackIndex];

                    if (attack.attacker.IsDead == false && attack.target.IsDead == false)
                    {
                        // Have both attacker and target attack
                        // Whoever is faster will attack first

                        int attackerSpeed = attack.attacker.stats.GetStat(Statistic.Type.Speed).Value;
                        int targetSpeed = attack.target.stats.GetStat(Statistic.Type.Speed).Value;

                        Pawn first = attack.attacker;
                        Pawn second = attack.target;

                        if (attackerSpeed < targetSpeed)
                        {
                            first = attack.target;
                            second = attack.attacker;
                        }

                        if (firstAttacked == false)
                        {
                            first.GetWeapon()?.Attack(second);
                            firstAttacked = true;
                            nextPawnAttackTime = Time.time + nextPawnAttackDelay;
                        }
                        else if (secondAttacked == false)
                        {
                            second.GetWeapon()?.Attack(first);
                            secondAttacked = true;
                            nextPawnAttackTime = Time.time + nextPawnAttackDelay;
                        }
                    }

                    if ((firstAttacked && secondAttacked) || (attack.attacker.IsDead || attack.target.IsDead))  // If attacks were made or cannot be done, go to next attack
                    {
                        // Increment to next attack
                        currentAttackIndex++;
                        firstAttacked = false;
                        secondAttacked = false;
                    }
                }
            }
        }
        public void StartAttacks(List<AttackInfo> attacks)
        {
            this.attacks = attacks;
            currentAttackIndex = 0;
            doAttacks = true;
        }
        void EndBattle()
        {
            onBattleFinished?.Invoke(this);
        }
        /// <summary>
        /// Gives control to the next party
        /// </summary>
        void NextTurn()
        {
            Debug.Log("NEXT TRUN");
            activePartyIndex++;
            if (activePartyIndex >= parties.Length)
                activePartyIndex = 0;

            OnPartyTurn?.Invoke(ActiveParty);
        }
        /// <summary>
        /// Data describing which pawn will attack who with what
        /// </summary>
        public struct AttackInfo
        {
            public AttackInfo(Pawn attacker, Pawn target)
            {
                this.attacker = attacker;
                this.target = target;
            }
            public Pawn attacker;
            public Pawn target;
        }
    }
}
