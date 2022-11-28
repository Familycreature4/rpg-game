using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CursorHelper : MonoBehaviour
{
    public void SetText(string text)
    {
        transform.Find("Text").GetComponent<Text>().text = text;
    }
    private void LateUpdate()
    {
        // Set rect transform size to text

        Text text = transform.Find("Text").GetComponent<Text>();
        Vector2 textSize = new Vector2(text.preferredWidth, text.preferredHeight);

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = textSize;

        rect.anchoredPosition = rect.worldToLocalMatrix * UnityEngine.Input.mousePosition;
    }
}
