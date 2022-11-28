using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Items
{
    [CreateAssetMenu(fileName = "Weapon Data", menuName = "RPG/Weapon Data")]
    public class Weapon : Item
    {
        public float baseDamage = 30.0f;
        public void Attack(Pawn target)
        {
            int damageStat = Owner.stats.GetStat( Statistic.Type.Strength ).Value;
            float damage = baseDamage * (Mathf.Log(damageStat, 10.0f) + 1); // Log -> slope decay as X goes to infinity
        }
        public static Weapon GetRandomWeapon()
        {
            Weapon[] weapons = Resources.LoadAll<Weapon>("Items");
            return weapons[UnityEngine.Random.Range(0, weapons.Length)];
        }
    }
}