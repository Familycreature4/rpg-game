using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public partial class Battle
    {
        /// <summary>
        /// Encapsulates party state in a battle
        /// </summary>
        public class BattleParty
        {
            public bool MyTurn => battle.ActiveParty == this;
            public bool AllDead
            {
                get
                {
                    foreach (Pawn pawn in party.pawns)
                    {
                        if (pawn.IsDead == false)
                            return false;
                    }

                    return true;
                }
            }
            public BattleParty(Party party, Battle battle)
            {
                this.party = party;
                this.battle = battle;
                Director.Current.OnBattlePartyTurn += OnPartyTurn;
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
                    attacks.Add(pawn, new AttackInfo(pawn, GetEnemyParty().GetRandomAlivePawn()));
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
            public Pawn GetRandomAlivePawn()
            {
                foreach (Pawn otherPawn in party.pawns)
                {
                    if (otherPawn.IsDead == false)
                        return otherPawn;
                }

                return null;
            }
            public void DebugAttacks()
            {
                foreach (KeyValuePair<Pawn, AttackInfo> pair in attacks)
                {
                    UnityEngine.Debug.DrawLine(pair.Value.attacker.TileTransform.coordinates + Vector3.one / 2.0f, pair.Value.target.TileTransform.coordinates + Vector3.one / 2.0f, Color.red);
                }
            }
            public virtual void OnPartyTurn(BattleParty party)
            {

            }
        }
    }
}
