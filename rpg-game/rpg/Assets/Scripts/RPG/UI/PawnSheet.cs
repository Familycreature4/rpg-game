using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RPG.UI
{
    public class PawnSheet : MonoBehaviour
    {
        Pawn pawn;

        public void SetPawn(Pawn pawn)
        {
            this.pawn = pawn;
            UpdateContents();
        }
        void UpdateContents()
        {
            transform.Find("Header/Info/Name").GetComponent<Text>().text = pawn.name;
            #region Stats
            Transform bodyStats = transform.Find("Body/Stats");
            foreach (Transform child in bodyStats)
            {
                GameObject.Destroy(child.gameObject);
            }
            
            foreach (Statistic stat in pawn.stats.GetStats())
            {
                GameObject statGO = Instantiate(Resources.Load<GameObject>("UI/Stat"), bodyStats);

                Text name = statGO.transform.Find("Name").GetComponent<Text>();
                Text value = statGO.transform.Find("Value").GetComponent<Text>();

                name.text = stat.type.ToString();
                value.text = stat.Value.ToString();
            }
            #endregion
        }
    }
}
