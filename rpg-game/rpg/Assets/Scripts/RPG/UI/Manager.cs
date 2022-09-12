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
        Party myParty;
        Party enemyParty;

        private void Awake()
        {
            if (current == null)
                current = this;

            myParty = GameObject.Instantiate(Resources.Load<GameObject>("UI/Party"), Manager.Canvas.transform).GetComponent<Party>();
            enemyParty = GameObject.Instantiate(Resources.Load<GameObject>("UI/Party"), Manager.Canvas.transform).GetComponent<Party>();

            myParty.Hide();
            enemyParty.Hide();

            Director.Current.OnBattleStart += OnBattleStart;
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
    }
}
