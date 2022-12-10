using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class PartyInfo : MonoBehaviour
    {
        public enum InfoType
        {
            Generic,
            Battle
        }
        Text PartyName => transform.Find("Canvas/PartyName").GetComponent<Text>();
        RPG.Party party;
        InfoType infoType;
        public void SetParty(RPG.Party party, InfoType type = InfoType.Generic)
        {
            this.party = party;
            this.infoType = type;

            UpdateContents();
        }

        void UpdateContents()
        {
            PartyName.text = party.name;

            Transform list = transform.Find("PawnList");
            for (int i = list.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(list.GetChild(i).gameObject);
            }

            GameObject prefab = Resources.Load<GameObject>("UI/PawnInfo");
            switch (infoType)
            {
                case InfoType.Battle:
                    prefab = Resources.Load<GameObject>("UI/BattlePawnInfo");
                    break;
                case 0:
                    prefab = Resources.Load<GameObject>("UI/PawnInfo");
                    break;
            }

            foreach (RPG.Pawn pawn in party.Pawns)
            {
                GameObject instance = GameObject.Instantiate(prefab, list);
                PawnInfo info = instance.GetComponent<PawnInfo>();
                info.SetPawn(pawn);
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
