using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class Party : MonoBehaviour
    {
        RectTransform Transform => GetComponent<RectTransform>();
        RPG.Party party;
        PawnStatus[] pawns;
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
            // Remove existing UI elements
            if (pawns != null)
            {
                for (int i = 0; i < pawns.Length; i++)
                {
                    GameObject.Destroy(pawns[i].gameObject);
                }
            }

            transform.Find("Header/Text").GetComponent<Text>().text = "PARTY NAME";

            pawns = new PawnStatus[party.Count];
            for (int i = 0; i < party.Count; i++)
            {
                pawns[i] = Instantiate(Resources.Load<GameObject>("UI/PawnStatus"), transform.Find("Pawns")).GetComponent<UI.PawnStatus>();
                pawns[i].SetPawn(party.pawns[i]);
            }
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
            }
            else
            {
                Transform.pivot = new Vector2(1.0f, 1.0f);
                Transform.anchorMin = new Vector2(1.0f, 1.0f);
                Transform.anchorMax = new Vector2(1.0f, 1.0f);
                Transform.anchoredPosition = new Vector2(-10, -10);
            }
        }
    }
}
