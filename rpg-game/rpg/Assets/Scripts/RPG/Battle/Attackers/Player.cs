using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace RPG.Battle.Attackers
{
    public class Player : Attacker
    {
        public Player(Party party, Battle battle, RPGPlayer player) : base(party, battle)
        {
            this.player = player;

            EventManager.OnSelect += OnGameObjectSelected;
            EventManager.OnInput += OnInput;
        }

        RPGPlayer player;
        public Pawn selectedPawn;
        public override void DeInit()
        {
            EventManager.OnInput -= OnInput;
        }
        public override void TurnBegin()
        {
            base.CreateRandomAttacks();
        }
        public override void TurnThink()
        {
            
        }
        public override void TurnEnd()
        {

        }
        void OnGameObjectSelected(RPG.Events.SelectArgs args)
        {
            Pawn pawn = args.Pawn;
            if (pawn != null)
            {
                if (pawn.party == player.Party)
                {
                    selectedPawn = pawn;
                    args.Use();
                }
                else if (selectedPawn != null && selectedPawn.party != pawn.party)
                {
                    // Create attack
                    base.targets[selectedPawn] = pawn;
                    selectedPawn = null;
                    args.Use();
                }
            }
            else if (selectedPawn != null && selectedPawn.party == player.Party)
            {
                Vector3Int coord = Vector3Int.FloorToInt(args.raycastHit.point + args.raycastHit.normal * 0.01f);
                float sqrDistance = (coord - selectedPawn.Coordinates).sqrMagnitude;
                // Try to place selected pawn down
                // && TileTools.GetPath(selectedPawn.Coordinates, coord, out List<Vector3Int> path, pawn.CanStandHere)
                if (sqrDistance <= Mathf.Pow(selectedPawn.battleMoveDistance, 2))
                {
                    battle.SetPawnPosition(selectedPawn, coord);
                    args.Use();
                    return;
                }
            }
        }
        public void OnInput(Input.RPGInput input)
        {
            if (MyTurn == false)
                return;

            if (input.confirm.Pressed)
            {
                battle.TryTurn(this);
            }
        }
    }
}
