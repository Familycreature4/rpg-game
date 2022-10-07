using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class SelectionHoverUI : MonoBehaviour
    {
        Text Text => transform.Find("Text").GetComponent<Text>();
        RectTransform CanvasRect => GetComponent<RectTransform>();
        private void Start()
        {
            Selector.Current.OnObjectStartHover += OnObjectStartHover;
            Selector.Current.OnObjectStopHover += OnObjectStopHover;
        }
        void OnObjectStartHover(Selector.Selection selection)
        {
            if (selection.gameObject.TryGetComponent<Pawn>(out Pawn pawn))
            {
                GetComponent<Canvas>().enabled = true;
                Text.text = $"{pawn.name}";
                CanvasRect.sizeDelta = new Vector2(Text.preferredWidth, Text.preferredHeight);
            }
        }
        void OnObjectStopHover()
        {
            GetComponent<Canvas>().enabled = false;
        }

        private void LateUpdate()
        {
            if (GetComponent<Canvas>().enabled)
            {
                CanvasRect.anchoredPosition = CanvasRect.worldToLocalMatrix * UnityEngine.Input.mousePosition;
            }
        }
    }
}
