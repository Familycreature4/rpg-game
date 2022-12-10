using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace RPG.Battle.Attackers
{
    public class Bot : Attacker
    {
        public Bot(Party party, Battle battle) : base(party, battle)
        {

        }

        float thinkDelay = 0.64f;
        float nextThinkTime;
        public override void DeInit()
        {
            
        }
        public override void TurnBegin()
        {
            nextThinkTime = Time.time + thinkDelay;
            base.CreateRandomAttacks();
        }

        public override void TurnThink()
        {
            if (Time.time >= nextThinkTime)
            {
                battle.TryTurn(this);
            }
        }

        public override void TurnEnd()
        {
            
        }
    }
}
