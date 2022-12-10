using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.UI
{
    public class PawnSheet : MonoBehaviour
    {
        PawnInfo PawnInfo => transform.Find("PawnInfo").gameObject.GetComponent<PawnInfo>();
        StatList StatList => transform.Find("StatList").gameObject.GetComponent<StatList>();
        Pawn pawn;
        public void SetPawn(Pawn pawn)
        {
            this.pawn = pawn;
            PawnInfo.SetPawn(pawn);
            StatList.SetStats(pawn.stats);

            UpdateContents();
        }

        void UpdateContents()
        {
            PawnInfo.UpdateContents();
            StatList.UpdateContents();

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
