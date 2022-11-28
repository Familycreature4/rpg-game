using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG.Battle.Attackers;
using UnityEngine;
namespace RPG.Battle
{
    public class BattleManager
    {
        public Party ActiveParty => ActiveAttacker.Party;
        Attacker ActiveAttacker => attackers [activeAttackerIndex];
        Attacker[] attackers;
        int activeAttackerIndex;
        public static BattleManager New(params Party[] parties)
        {
            BattleManager manager = new BattleManager();
            manager.attackers = new Attacker[parties.Length];
            int playerIndex = 0;
            for (int i = 0; i < parties.Length; i++)
            {
                Party party = parties[i];
                if (party.IsPlayer)
                {
                    manager.attackers[i] = new Attackers.Player(party, manager, Player.Current as RPGPlayer);
                    playerIndex = i;
                }
                else
                    manager.attackers[i] = new Attackers.Bot(party, manager);
            }

            manager.activeAttackerIndex = playerIndex;

            return manager;
        }
        public void TryTurn(Attacker attacker)
        {
            if (attacker == ActiveAttacker)
            {
                Debug.Log($"{attacker} has attacked!!!");

                activeAttackerIndex++;
                if (activeAttackerIndex >= attackers.Length)
                    activeAttackerIndex = 0;

                ActiveAttacker.TurnBegin();
            }
        }
        public void Update()
        {
            // Get and update the active attacker

            ActiveAttacker?.TurnThink();
        }
    }
}
