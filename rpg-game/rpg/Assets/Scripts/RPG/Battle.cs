using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Items;
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
            ActiveParty.DebugAttacks();
        }
        /// <summary>
        /// Submits a party's attacks and executes them
        /// </summary>
        public void PartyAttack()
        {
            foreach (AttackInfo attack in ActiveParty.Attacks)
            {
                Weapon weapon = attack.attacker.GetWeapon();
                if (weapon != null)
                {
                    weapon.Attack(attack.target);
                }
            }

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
            public DamageInfo Damage => new DamageInfo { attacker = attacker, damage = UnityEngine.Random.Range(1.0f, 25.0f), victim = target };
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
            public IEnumerable Attacks
            {
                get
                {
                    foreach (KeyValuePair<Pawn, AttackInfo> pair in attacks)
                    {
                        yield return pair.Value;
                    }
                }
            }
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
            public void DebugAttacks()
            {
                foreach (KeyValuePair<Pawn, AttackInfo> pair in attacks)
                {
                    UnityEngine.Debug.DrawLine(pair.Value.attacker.TileTransform.coordinates + Vector3.one / 2.0f, pair.Value.target.TileTransform.coordinates + Vector3.one / 2.0f, Color.red);
                }
            }
        }
        /// <summary>
        /// BattleParty whose attacks are made by the client
        /// </summary>
        public class ClientBattle : BattleParty
        {
            public ClientBattle(Party p, Battle b) : base(p, b)
            {
                Selector.Current.OnObjectSelect += OnObjectSelect;
            }
            Pawn targetPawn;
            Pawn attackerPawn;
            public override void TurnThink()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ExecuteAttacks();
                }
            }
            void OnObjectSelect(Selector.Selection selection)
            {
                if (MyTurn == false)
                    return;

                if (selection.gameObject.TryGetComponent<Pawn>(out Pawn pawn))
                {
                    if (pawn.party == this.party)
                    {
                        attackerPawn = pawn;
                    }
                    else
                    {
                        targetPawn = pawn;
                    }

                    if (attackerPawn != null && targetPawn != null)
                    {
                        // Create attack
                        Debug.Log($"{attackerPawn.name} attacking {targetPawn.name}");
                        attacks[attackerPawn] = new AttackInfo { attacker = attackerPawn, target = targetPawn };
                        attackerPawn = null;
                        targetPawn = null;
                    }
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
