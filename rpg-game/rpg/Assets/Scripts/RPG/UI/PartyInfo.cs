using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI
{
    public class PartyInfo : MonoBehaviour
    {
        RPG.Party party;
        public void SetParty(RPG.Party party)
        {
            this.party = party;

            UpdateContents();
        }

        void UpdateContents()
        {
            Transform list = transform.Find("PawnList");
            for (int i = list.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(list.GetChild(i).gameObject);
            }

            foreach (RPG.Pawn pawn in party.pawns)
            {
                GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>("UI/PawnInfo"), transform.Find("PawnList"));
                PawnInfo info = instance.GetComponent<PawnInfo>();
                info.SetPawn(pawn);
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            GetComponent<Canvas>().enabled = true;
        }
    }
}
