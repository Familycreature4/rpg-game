using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Battle.Attackers
{
    public class Player : Attacker, Input.IInputReceiver
    {
        public Player(Party party, BattleManager battle, RPGPlayer player) : base(party, battle)
        {
            this.player = player;

            player.input.Subscribe(this);
        }

        RPGPlayer player;

        public override void TurnBegin()
        {
            
        }

        public override void TurnThink()
        {
            
        }

        public void OnInputReceived(Input.Input input)
        {
            Input.RPGInput rpgInput = input as Input.RPGInput; 
            if (rpgInput.confirm.Pressed)
            {
                battle.TryTurn(this);
            }
        }
        public int GetInputPriority()
        {
            return 0;
        }
    }
}
