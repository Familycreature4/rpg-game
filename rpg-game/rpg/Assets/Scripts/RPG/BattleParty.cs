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
                List<AttackInfo> attacks = new List<AttackInfo>(this.attacks.Count);
                foreach (AttackInfo a in Attacks)
                    attacks.Add(a);
                battle.StartAttacks(attacks);
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
                if (AllDead)
                    return null;

                while (true)
                {
                    Pawn pawn = party.pawns[UnityEngine.Random.Range(0, party.Count)];
                    if (pawn.IsDead == false)
                        return pawn;
                }

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
