using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace RPG.UI
{
    public class PawnInfo : MonoBehaviour, UnityEngine.EventSystems.IPointerClickHandler, UnityEngine.EventSystems.IPointerEnterHandler, UnityEngine.EventSystems.IPointerExitHandler
    {
        protected RPG.Pawn pawn;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (pawn != null)
            {
                RPG.Events.SelectArgs args = new Events.SelectArgs(pawn);
                EventManager.OnSelect?.Invoke(args);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            RPG.Events.SelectArgs args = new Events.SelectArgs(pawn);
            EventManager.OnStartHover?.Invoke(args);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventManager.OnEndHover?.Invoke(new RPG.Events.SelectArgs(pawn));
        }

        public virtual void SetPawn(RPG.Pawn pawn)
        {
            this.pawn = pawn;

            UpdateContents();
        }
        public virtual void UpdateContents()
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
                image.sprite = pawn.image;
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}
