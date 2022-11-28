using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class PawnInfo : MonoBehaviour
    {
        RPG.Pawn pawn;
        public void SetPawn(RPG.Pawn pawn)
        {
            this.pawn = pawn;

            UpdateContents();
        }
        public void UpdateContents()
        {
            Text text = transform.Find("Name").GetComponent<Text>();
            Image image = transform.Find("Image").GetComponent<Image>();

            if (pawn == null)
            {
                text.text = "NULL";
            }
            else
            {
                text.text = pawn.name;
            }

            GetComponent<Canvas>().enabled = true;
        }
    }
}
