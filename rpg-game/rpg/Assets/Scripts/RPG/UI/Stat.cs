using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;
using UnityEngine.UI;
namespace RPG.UI
{
    public class Stat : MonoBehaviour
    {
        public Text ValueText => transform.Find("Value").GetComponent<Text>();
        RPG.Statistic stat;
        public void SetStat(Statistic stat)
        {
            this.stat = stat;

            UpdateContent();
        }
        void UpdateContent()
        {
            Text textName = transform.Find("Name").GetComponent<Text>();
            Text textValue = transform.Find("Value").GetComponent<Text>();

            if (stat == null)
            {
                textName.text = "NULL";
                textValue.text = "NULL";
            }
            else
            {
                textName.text = stat.GetName();
                textValue.text = stat.Value.ToString();
            }
        }
    }

}
