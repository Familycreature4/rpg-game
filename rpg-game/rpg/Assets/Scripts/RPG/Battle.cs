using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace RPG
{
    /// <summary>
    /// Stores the state of a battle among parties
    /// </summary>
    public class Battle
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
                    battleParty = new ClientBattle(party, this);
                else
                    battleParty = new BotBattle(parties[i], this);

                this.parties[i] = battleParty;
            }
        }
        public BattleParty ActiveParty => parties[activePartyIndex];
        public Pawn ActivePawn => ActiveParty.party.pawns[activePawnIndex];
        public BattleParty[] parties;
        int activePartyIndex;
        int activePawnIndex;
        public State state;
        public Action<BattleParty> OnPartyTurn;  // Invoked when the active party is active
        public Action<BattleParty> OnPartyAttack;  // Invoked when the active party begins attack / Ends planning
        public void Update()
        {
            ActiveParty.TurnThink();
        }
        /// <summary>
        /// Submits a party's attacks and executes them
        /// </summary>
        public void PartyAttack()
        {
            OnPartyAttack?.Invoke(ActiveParty);
            NextTurn();
        }
        /// <summary>
        /// Gives control to the next party
        /// </summary>
        void NextTurn()
        {
            activePartyIndex++;
            if (activePartyIndex >= parties.Length)
                activePartyIndex = 0;

            OnPartyTurn?.Invoke(ActiveParty);
            Debug.Log($"It is now {ActiveParty}'s turn");
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
        /// <summary>
        /// Encapsulates party state in a battle
        /// </summary>
        public class BattleParty
        {
            public bool MyTurn => battle.ActiveParty == this;
            public BattleParty(Party party, Battle battle)
            {
                this.party = party;
                this.battle = battle;
            }
            protected Battle battle;
            public readonly Party party;
            protected Dictionary<Pawn, AttackInfo> attacks = new Dictionary<Pawn, AttackInfo>();
            public void ExecuteAttacks()
            {
                battle.PartyAttack();
            }
            /// <summary>
            /// Called when it is the party's turn. Gives control to next party
            /// </summary>
            public virtual void TurnThink()
            {
                attacks.Clear();

                foreach (Pawn pawn in party.pawns)
                {
                    attacks.Add(pawn, new AttackInfo(pawn, GetEnemyParty().GetRandomPawn()));
                }

                ExecuteAttacks();
            }
            protected BattleParty GetEnemyParty()
            {
                foreach (BattleParty battleParty in battle.parties)
                {
                    if (battleParty != this)
                        return battleParty;
                }

                return null;
            }
            public Pawn GetRandomPawn()
            {
                return party.pawns[UnityEngine.Random.Range(0, party.pawns.Count)];
            }
        }
        /// <summary>
        /// BattleParty whose attacks are made by the client
        /// </summary>
        public class ClientBattle : BattleParty
        {
            public ClientBattle(Party p, Battle b) : base(p, b)
            {
                
            }
            public override void TurnThink()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    attacks.Clear();

                    foreach (Pawn pawn in party.pawns)
                    {
                        attacks.Add(pawn, new AttackInfo(pawn, GetEnemyParty().GetRandomPawn()));
                    }

                    ExecuteAttacks();
                }
            }
        }
        public class BotBattle : BattleParty
        {
            public BotBattle(Party p, Battle b) : base(p, b)
            {
                Director.Current.OnBattlePartyTurn += OnPartyTurn;
            }
            bool CanAttack => Time.time >= nextAttackTime;
            float nextAttackTime = 0.0f;
            float attackDelay = 1.0f;
            public override void TurnThink()
            {
                if (CanAttack)
                {
                    nextAttackTime = Time.time + attackDelay;

                    attacks.Clear();

                    foreach (Pawn pawn in party.pawns)
                    {
                        attacks.Add(pawn, new AttackInfo(pawn, GetEnemyParty().GetRandomPawn()));
                    }

                    ExecuteAttacks();
                }
            }
            void OnPartyTurn(BattleParty battleParty)
            {
                if (battleParty == this)
                    nextAttackTime = Time.time + attackDelay;
            }
        }
    }
}
