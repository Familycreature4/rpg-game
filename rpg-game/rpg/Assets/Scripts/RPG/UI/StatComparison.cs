using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG;
namespace RPG.UI
{
    public class StatComparison : MonoBehaviour
    {
        Stats leftStats;
        Stats rightStats;
        public void SetStats(Stats leftSide, Stats rightSide)
        {
            leftStats = leftSide;
            rightStats = rightSide;

            UpdateContents();
        }
        void UpdateContents()
        {
            void UpdateStats(Transform transform, Stats myStats, Stats otherStats)
            {
                transform.DeleteChildren();

                foreach (Statistic stat in myStats.GetStats())
                {
                    GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>("UI/Stat"), transform);
                    UI.Stat uiStat = instance.GetComponent<UI.Stat>();
                    uiStat.SetStat(stat);

                    // Get the correlating stat of other stats
                    Statistic otherStat = otherStats.GetStat(stat.type);

                    Color color = Color.white;
                    if (stat.Value > otherStat.Value)
                    {
                        color = Color.green;
                    }
                    else if (stat.Value < otherStat.Value)
                    {
                        color = Color.red;
                    }

                    uiStat.ValueText.color = color;
                }
            }

            UpdateStats(this.transform.Find("Stats/LeftStats"), leftStats, rightStats);
            UpdateStats(this.transform.Find("Stats/RightStats"), rightStats, leftStats);

            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            GetComponent<Canvas>().enabled = true;
        }
    }
}
