using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class Party : MonoBehaviour
    {
        RectTransform Transform => GetComponent<RectTransform>();
        Text PartyNameText => transform.GetChild(0).GetComponent<Text>();
        Text PartyPawnsText => transform.GetChild(1).GetComponent<Text>();
        RPG.Party party;
        private void Awake()
        {
            Canvas mainCanvas = Manager.Canvas;
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
        public void Hide()
        {
            GetComponent<Canvas>().enabled = false;
        }
        public void Show()
        {
            GetComponent<Canvas>().enabled = true;
        }
        public void SetSide(bool left = true)
        {
            if (left)
            {
                Transform.pivot = new Vector2(0.0f, 1.0f);
                Transform.anchorMin = new Vector2(0.0f, 1.0f);
                Transform.anchorMax = new Vector2(0.0f, 1.0f);
                Transform.anchoredPosition = new Vector2(10, -10);
                PartyNameText.alignment = TextAnchor.UpperLeft;
                PartyPawnsText.alignment = TextAnchor.UpperLeft;
            }
            else
            {
                Transform.pivot = new Vector2(1.0f, 1.0f);
                Transform.anchorMin = new Vector2(1.0f, 1.0f);
                Transform.anchorMax = new Vector2(1.0f, 1.0f);
                Transform.anchoredPosition = new Vector2(-10, -10);
                PartyNameText.alignment = TextAnchor.UpperRight;
                PartyPawnsText.alignment = TextAnchor.UpperRight;
            }
        }
    }
}
