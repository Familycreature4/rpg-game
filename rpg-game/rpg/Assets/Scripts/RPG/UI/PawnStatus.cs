using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class PawnStatus : MonoBehaviour
    {
        Text PawnName => transform.Find("PawnName").GetComponent<Text>();
        Text DamageText => transform.Find("PawnHealth").GetComponent<Text>();
        Pawn pawn;
        public void SetPawn(Pawn pawn)
        {
            if (this.pawn != null)
            {
                this.pawn.OnDamageTaken -= OnDamage;
            }

            this.pawn = pawn;
            this.pawn.OnDamageTaken += OnDamage;
            UpdateContents();
        }
        void UpdateContents()
        {
            PawnName.text = pawn.name;

            DamageText.text = $"{pawn.health.ToString("0")}";
            float per = pawn.health / pawn.maxHealth;
            Color color = pawn.health <= 0 ? Color.red : Color.Lerp(Color.yellow, Color.white, per);
            DamageText.color = color;
        }
        void OnDamage(DamageInfo damage)
        {
            UpdateContents();
        }
    }
}
