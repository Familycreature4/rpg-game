using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Items
{
    [CreateAssetMenu(fileName="Item Data", menuName ="RPG/Item Data")]
    public class Item : ScriptableObject
    {
        public Item()
        {

        }
        public Item(Pawn owner)
        {

        }
        public Sprite icon;
        public Inventory inventory;
    }
}
