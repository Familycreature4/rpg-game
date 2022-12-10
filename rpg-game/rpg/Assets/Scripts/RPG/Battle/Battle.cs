using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPG.Battle.Attackers;
using UnityEngine;
namespace RPG.Battle
{
    public class Battle
    {
        public enum State
        {
            Active,
            Finished
        }
        public Party ActiveParty => ActiveAttacker.Party;
        public Attacker ActiveAttacker => attackers [activeAttackerIndex];
        public Attacker[] attackers;
        Dictionary<Pawn, Vector3Int> pawnTargetCoords;  // Positions the pawns should go to
        int activeAttackerIndex;
        State state;
        public static Battle New(params Party[] parties)
        {
            Battle battle = new Battle();
            battle.state = State.Active;
            battle.attackers = new Attacker[parties.Length];
            int playerIndex = 0;
            for (int i = 0; i < parties.Length; i++)
            {
                Party party = parties[i];
                if (party.IsPlayer)
                {
                    battle.attackers[i] = new Attackers.Player(party, battle, Player.Current as RPGPlayer);
                    playerIndex = i;
                }
                else
                    battle.attackers[i] = new Attackers.Bot(party, battle);
            }

            battle.activeAttackerIndex = playerIndex;

            // Initialize pawn coords
            battle.pawnTargetCoords = new Dictionary<Pawn, Vector3Int>();
            foreach (Party party in parties)
            {
                foreach (Pawn pawn in party.Pawns)
                {
                    battle.pawnTargetCoords[pawn] = pawn.Coordinates;
                    // Add move to order
                    Orders.MoveTo moveOrder = new Orders.MoveTo(pawn, null, false, -1);
                    moveOrder.PositionGetter = () =>
                    {
                        if (battle == null || battle.state == State.Finished)
                        {
                            moveOrder.IsComplete = true;
                            return pawn.Coordinates;
                        }
                        else
                        {
                            return battle.pawnTargetCoords[pawn];
                        }
                    };

                    pawn.Orders.Add(moveOrder);
                }
            }

            battle.ActiveAttacker.TurnBegin();

            return battle;
        }
        public void TryTurn(Attacker attacker)
        {
            if (attacker == ActiveAttacker)
            {
                // Attack logic

                foreach (KeyValuePair<Pawn, Pawn> pair in attacker.Targets)
                {
                    pair.Value.Damage(34);
                }

                ActiveAttacker.TurnEnd();
                EventManager.onAttackerTurnEnd?.Invoke(this, ActiveAttacker);

                // Assess whether or not this party is the last one
                bool allEnemiesDead = attackers.All( (Attacker otherAttacker) => { return otherAttacker == attacker ? true : otherAttacker.AllDead; });

                if (allEnemiesDead)  // Clean up battle
                {
                    state = State.Finished;

                    foreach (Attacker a in attackers)
                        a.DeInit();

                    EventManager.onBattleEnd?.Invoke(this);
                    return;
                }

                activeAttackerIndex++;
                if (activeAttackerIndex >= attackers.Length)
                    activeAttackerIndex = 0;

                ActiveAttacker.TurnBegin();
                EventManager.onAttackerTurnStart?.Invoke(this, ActiveAttacker);
            }
        }
        public void SetPawnPosition(Pawn pawn, Vector3Int coords)
        {
            pawnTargetCoords[pawn] = coords;
        }
        public Vector3Int GetPawnPosition(Pawn pawn) => pawnTargetCoords[pawn];
        public void Update()
        {
            // Get and update the active attacker

            ActiveAttacker?.TurnThink();
        }
        public Attackers.Attacker GetAttacker(Party party)
        {
            foreach (Attackers.Attacker attacker in attackers)
            {
                if (attacker.Party == party)
                    return attacker;
            }

            return null;
        }
        public Pawn GetPawnTarget(Pawn attackerPawn)
        {
            foreach (Attacker attacker in attackers)
            {
                if (attacker.Party.Contains(attackerPawn) && attacker.Targets.TryGetValue(attackerPawn, out Pawn otherPawn))
                {
                    return otherPawn;
                }
            }

            return null;
        }
    }
}
