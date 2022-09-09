using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class BattleUI : MonoBehaviour
    {
        Text TurnText => transform.Find("Turn/Turn Text").GetComponent<Text>();
        private void Awake()
        {
            Director.Current.OnBattlePartyTurn += OnPartyTurn;
            Director.Current.OnBattleStart += OnBattleStart;
            GetComponent<Canvas>().enabled = false;
        }
        void OnPartyTurn(Battle.BattleParty battleParty)
        {
            string text = battleParty.party.IsClient ? "YOUR TURN" : "ENEMY'S TURN";
            TurnText.text = text;
        }
        void OnBattleStart(Battle battle)
        {
            GetComponent<Canvas>().enabled = true;
        }
    }
}
