using UnityEngine;
namespace RPG
{
    public partial class Battle
    {
        /// <summary>
        /// BattleParty whose attacks are made by the client
        /// </summary>
        public class ClientBattle : BattleParty
        {
            const string idleMessage = "Select a pawn of your party to plan an attack!\nPress SPACE BAR to execute attacks";
            public ClientBattle(Party p, Battle b) : base(p, b)
            {
                Selector.Current.OnObjectSelect += OnObjectSelect;
            }
            Pawn targetPawn;
            Pawn attackerPawn;
            public override void TurnThink()
            {
                if (Client.Current.input.confirm.Value)
                {
                    ExecuteAttacks();
                }
            }
            void OnObjectSelect(Selector.Selection selection)
            {
                if (MyTurn == false)
                    return;

                if (selection.gameObject.TryGetComponent<Pawn>(out Pawn pawn))
                {
                    if (pawn.party == this.party)
                    {
                        attackerPawn = pawn;
                        UI.Manager.Current.infoHelper.SetText($"Select an enemy pawn to attack using {attackerPawn.name}!");
                    }
                    else if (attackerPawn != null)
                    {
                        targetPawn = pawn;
                        UI.Manager.Current.infoHelper.SetText(idleMessage);
                    }

                    if (attackerPawn != null && targetPawn != null)
                    {
                        // Create attack
                        Debug.Log($"{attackerPawn.name} attacking {targetPawn.name}");
                        attacks[attackerPawn] = new AttackInfo { attacker = attackerPawn, target = targetPawn };
                        attackerPawn = null;
                        targetPawn = null;
                        
                    }
                }
            }
            public override void OnPartyTurn(BattleParty party)
            {
                if (party.party.IsClient)
                {
                    UI.Manager.Current.infoHelper.SetText(idleMessage);
                }
                else
                {
                    UI.Manager.Current.infoHelper.Hide();
                }
               
            }
        }
    }
}
