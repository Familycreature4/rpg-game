using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class Party : MonoBehaviour
    {
        Text PartyNameText => transform.GetChild(0).GetComponent<Text>();
        Text PartyPawnsText => transform.GetChild(1).GetComponent<Text>();
        RPG.Party party;
        private void Awake()
        {
            GameObject mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
            if (mainCanvas)
            {
                ((RectTransform)transform).SetParent(mainCanvas.transform);
            }
        }
        public void SetParty(RPG.Party party)
        {
            this.party = party;
            UpdateContents();
        }
        void UpdateContents()
        {
            PartyNameText.text = "PARTY :)";
            string pawns = "";
            foreach (Pawn pawn in party.pawns)
            {
                pawns += pawn.name + "\n";
            }
            PartyPawnsText.text = pawns;
        }
    }
}
