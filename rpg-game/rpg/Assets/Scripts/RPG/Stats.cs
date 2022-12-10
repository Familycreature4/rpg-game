using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace RPG
{
    /// <summary>
    /// Manager class for handling a pawn's stats
    /// </summary>
    public class Stats
    {
        public Stats()
        {
            stats = new Dictionary<Statistic.Type, Statistic>();

            foreach (Statistic.Type statType in Enum.GetValues(typeof(Statistic.Type)))
            {
                stats.Add(statType, new Statistic(statType, UnityEngine.Random.Range(0, 100)));
            }
        }
        Dictionary<Statistic.Type, Statistic> stats;
        System.Action<Statistic> onStatisticChange;
        public Statistic GetStat(Statistic.Type type)
        {
            return stats[type];
        }
        public Statistic GetStat(string type)
        {
            return stats[Enum.Parse<Statistic.Type>(type)];
        }
        public IEnumerable<Statistic> GetStats()
        {
            foreach (KeyValuePair<Statistic.Type, Statistic> pair in stats)
            {
                yield return pair.Value;
            }
        }

        public static Stats FromJson(JObject json)
        {
            Stats stats = new Stats();  // This initializes every stat

            foreach (JProperty property in json.Properties())
            {
                stats.GetStat(property.Name).Value = (int)property.Value;
            }

            return stats;
        }
    }
}
