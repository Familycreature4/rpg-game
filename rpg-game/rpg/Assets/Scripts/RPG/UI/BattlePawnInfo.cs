using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class BattlePawnInfo : PawnInfo
    {
        private void Awake()
        {
            EventManager.onPawnDamaged += OnTakeDamage;
        }
        public Text HealthText => transform.Find("Info/Health").GetComponent<Text>();
        public override void SetPawn(Pawn pawn)
        {
            base.SetPawn(pawn);
        }
        public override void UpdateContents()
        {
            Text name = transform.Find("Info/Name").GetComponent<Text>();
            Image image = transform.Find("Image").GetComponent<Image>();

            if (pawn == null)
            {
                name.text = "NULL";
            }
            else
            {
                name.text = pawn.name;
                HealthText.text = pawn.Health.ToString();
                image.sprite = pawn.image;
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
        void OnTakeDamage(Pawn pawn, int amount)
        {
            if (pawn == this.pawn)
                HealthText.text = pawn.Health.ToString();
        }
        private void OnDestroy()
        {
            EventManager.onPawnDamaged -= OnTakeDamage;
        }
    }
}
