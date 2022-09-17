using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class InfoHelper : MonoBehaviour
    {
        Text Text => transform.Find("Text").GetComponent<Text>();
        public void SetText(string text)
        {
            GetComponent<Canvas>().enabled = true;
            Text.text = text;
        }
        public void SetText()
        {
            GetComponent<Canvas>().enabled = false;
        }
        public void Hide()
        {
            SetText();
        }
    }
}
