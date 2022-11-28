using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;
namespace RPG.UI
{
    public class StatList : MonoBehaviour
    {
        Stats stats;
        public void SetStats(Stats stats)
        {
            this.stats = stats;

            UpdateContents();
        }
        public void UpdateContents()
        {
            // Clear existing stats
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }


            // Add new stats
            foreach (Statistic stat in stats.GetStats())
            {
                GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>("UI/Stat"), transform);
                UI.Stat uiStat = instance.GetComponent<UI.Stat>();
                uiStat.SetStat(stat);
            }

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            GetComponent<Canvas>().enabled = true;
        }
    }
}
