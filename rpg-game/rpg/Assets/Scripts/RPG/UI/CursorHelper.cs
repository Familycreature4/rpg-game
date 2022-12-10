using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CursorHelper : MonoBehaviour
{
    public bool Visible
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            gameObject.SetActive(value);
        }
    }
    Selector selector;
    public void SetText(string text)
    {
        transform.Find("Text").GetComponent<Text>().text = text;
    }
    private void Update()
    {
        if (selector == null)
        {
            if (Player.Current != null && Player.Current is RPG.RPGPlayer rpgPlayer)
            {
                selector = rpgPlayer.selector;
                EventManager.OnStartHover += OnGameObjectStartHover;
            }
        }
    }
    private void LateUpdate()
    {
        GameObject currentHover = selector.currentHover;
        if (currentHover == null)
        {
            Visible = false;
            return;
        }
        else
        {
            Visible = true;
            // Set rect transform size to text

            Text text = transform.Find("Text").GetComponent<Text>();
            Vector2 textSize = new Vector2(text.preferredWidth, text.preferredHeight);

            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = textSize;

            rect.anchoredPosition = rect.worldToLocalMatrix * UnityEngine.Input.mousePosition;
        }
    }
    void OnGameObjectStartHover(RPG.Events.SelectArgs args)
    {
        if (args.IsPawn)
        {
            Visible = true;
            SetText(args.Pawn.name);
        }
        else
        {
            Visible = false;
        }
    }
}
