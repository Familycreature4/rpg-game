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
            target.TakeDamage(new DamageInfo { attacker = inventory.owner, damage = baseDamage, victim = target });
        }
        public static Weapon GetRandomWeapon()
        {
            Weapon[] weapons = Resources.LoadAll<Weapon>("Items");
            return weapons[UnityEngine.Random.Range(0, weapons.Length)];
        }
    }
}