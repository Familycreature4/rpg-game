using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Battle.Attackers
{
    public abstract class Attacker
    {
        public Attacker(Party party, Battle battle)
        {
            this.Party = party;
            this.battle = battle;
            targets = new Dictionary<Pawn, Pawn>();
        }
        public Party Party { get; private set; }
        public Dictionary<Pawn, Pawn> Targets => targets;
        public bool AllDead
        {
            get
            {
                return Party.Pawns.All((Pawn pawn) => { return pawn.IsAlive == false; } );
            }
        }
        protected bool MyTurn => battle.ActiveAttacker == this;
        protected Battle battle;
        protected Dictionary<Pawn, Pawn> targets;
        public abstract void TurnThink();
        public abstract void TurnBegin();
        public abstract void TurnEnd();
        public abstract void DeInit();
        protected void CreateRandomAttacks()
        {
            targets.Clear();
            foreach (Pawn pawn in Party.Pawns.Where(( Pawn pawn )=> { return pawn.IsAlive; }))
            {
                Pawn enemyPawn = GetRandomEnemyPawn();
                if (enemyPawn != null)
                    targets[pawn] = enemyPawn;
            }
        }
        protected Attacker GetRandomEnemyAttacker()
        {
            List<Attacker> validAttackers = new List<Attacker>();
            foreach (Attacker attacker in battle.attackers)
            {
                if (attacker != this && attacker.AllDead == false)
                    validAttackers.Add(attacker);
                    
            }

            if (validAttackers.Count > 0)
                return validAttackers[UnityEngine.Random.Range(0, validAttackers.Count)];

            return null;
        }
        protected Pawn GetRandomEnemyPawn()
        {
            Attacker attacker = GetRandomEnemyAttacker();
            if (attacker != null)
                return attacker.GetRandomAlivePawn();

            return null;
        }
        protected Pawn GetRandomAlivePawn()
        {
            List<Pawn> validPawns = new List<Pawn>();
            foreach (Pawn pawn in Party.Pawns)
            {
                if (pawn.IsAlive)
                    validPawns.Add(pawn);
            }

            if (validPawns.Count > 0)
                return validPawns[UnityEngine.Random.Range(0, validPawns.Count)];

            return null;
        }
    }
}
