using UnityEngine;
namespace RPG
{
    public partial class Battle
    {
        public class BotBattle : BattleParty
        {
            public BotBattle(Party p, Battle b) : base(p, b)
            {
                
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
                        if (pawn.IsDead)
                            continue;

                        Pawn target = GetEnemyParty().GetRandomAlivePawn();
                        if (target != null)
                        {
                            attacks.Add(pawn, new AttackInfo(pawn, target));
                        }
                    }

                    ExecuteAttacks();
                }
            }
            override public void OnPartyTurn(BattleParty battleParty)
            {
                if (battleParty == this)
                    nextAttackTime = Time.time + attackDelay;
            }
        }
    }
}
