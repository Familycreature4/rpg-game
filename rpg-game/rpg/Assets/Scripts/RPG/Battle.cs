using System;
using RPG.Items;
namespace RPG
{
    /// <summary>
    /// Stores the state of a battle among parties
    /// </summary>
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
                    battleParty = new ClientBattle(party, this);
                else
                    battleParty = new BotBattle(parties[i], this);

                this.parties[i] = battleParty;
            }

            OnPartyTurn?.Invoke(ActiveParty);
        }
        public BattleParty ActiveParty => parties[activePartyIndex];
        public Pawn ActivePawn => ActiveParty.party.pawns[activePawnIndex];
        public BattleParty[] parties;
        int activePartyIndex;
        int activePawnIndex;
        public State state;
        public Action<BattleParty> OnPartyTurn;  // Invoked when the active party is active
        public Action<BattleParty> OnPartyAttack;  // Invoked when the active party begins attack / Ends planning
        public Action<Battle> onBattleFinished;
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
                if (attack.attacker.IsDead)
                    continue;

                // Have both attacker and target attack
                // Whoever is faster will attack first

                int attackerSpeed = attack.attacker.stats.GetStat( Statistic.Type.Speed ).Value;
                int targetSpeed = attack.target.stats.GetStat(Statistic.Type.Speed).Value;

                Pawn first = attack.attacker;
                Pawn second = attack.target;

                if (attackerSpeed < targetSpeed)
                {
                    first = attack.target;
                    second = attack.attacker;
                }

                first.GetWeapon()?.Attack(second);

                if (second.IsDead)
                    continue;

                second.GetWeapon()?.Attack(first);
            }

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
        void EndBattle()
        {
            onBattleFinished?.Invoke(this);
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
