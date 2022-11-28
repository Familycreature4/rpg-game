using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Battle.Attackers
{
    public abstract class Attacker
    {
        public Attacker(Party party, BattleManager battle)
        {
            this.Party = party;
            this.battle = battle;
        }
        public Party Party { get; private set; }
        protected BattleManager battle;
        public abstract void TurnThink();
        public abstract void TurnBegin();
    }
}
