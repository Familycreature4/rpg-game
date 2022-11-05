using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.UI
{
    /// <summary>
    /// Manages the UI layout
    /// </summary>
    public class Manager : MonoBehaviour
    {
        public static Manager Current => current;
        public static Canvas Canvas => Current.GetComponent<Canvas>();
        static Manager current;
        public InfoHelper infoHelper;
        PawnSheet sheet;
        Party myParty;
        Party enemyParty;

        private void Awake()
        {
            if (current == null)
                current = this;

            myParty = GameObject.Instantiate(Resources.Load<GameObject>("UI/Party"), Manager.Canvas.transform).GetComponent<Party>();
            myParty.Hide();
            enemyParty = GameObject.Instantiate(Resources.Load<GameObject>("UI/Party"), Manager.Canvas.transform).GetComponent<Party>();
            enemyParty.Hide();
            infoHelper = GameObject.Instantiate(Resources.Load<GameObject>("UI/InfoHelper"), Manager.Canvas.transform).GetComponent<InfoHelper>();
            infoHelper.Hide();
            sheet = GameObject.Instantiate(Resources.Load<GameObject>("UI/PawnSheet"), Manager.Canvas.transform).GetComponent<PawnSheet>();
            sheet.GetComponent<Canvas>().enabled = false;

            Director.Current.OnBattleStart += OnBattleStart;
            myParty.onPawnSelect += OnPawnSelect;
            enemyParty.onPawnSelect += OnPawnSelect;
            Selector.Current.OnObjectSelect += OnObjectSelect;
        }

        void OnBattleStart(Battle battle)
        {
            foreach (Battle.BattleParty p in battle.parties)
            {
                Party partyUI = enemyParty;

                bool clientParty = p.party == Client.Current.party;
                if (p.party == Client.Current.party)
                {
                    partyUI = myParty;
                }

                partyUI.SetSide(clientParty);
                partyUI.SetParty(p.party);
                partyUI.Show();
            }
        }
        void OnPawnSelect(RPG.Party party, Pawn pawn)
        {
            Selector.Current.OnObjectSelect?.Invoke(new Selector.Selection { gameObject = pawn.gameObject });
        }
        void OnObjectSelect(Selector.Selection selection)
        {
            if (selection.gameObject.TryGetComponent<Pawn>(out Pawn pawn))
            {
                sheet.SetPawn(pawn);
                sheet.GetComponent<Canvas>().enabled = true;
                AudioManager.Current.PlayOneShot(AudioSet.GetRandomClip("Page Flip"));
            }
        }
    }
}
