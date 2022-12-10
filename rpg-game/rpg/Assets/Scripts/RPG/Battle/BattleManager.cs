using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG.Battle
{
    public class BattleManager
    {
        public Party ActiveParty => currentBattle.ActiveParty;
        public Battle currentBattle;
        public void BeginBattle(params Party[] parties)
        {
            Battle newBattle = Battle.New(parties);
            currentBattle = newBattle;

            EventManager.onBattleEnd += OnBattleEnd;

            EventManager.onBattleStart?.Invoke(newBattle);
        }
        void OnBattleEnd(Battle battle)
        {
            currentBattle = null;
        }
        public void Update()
        {
            currentBattle?.Update();
        }
    }
}
