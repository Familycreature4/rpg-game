using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Items
{
    public class Inventory
    {
        public Inventory()
        {

        }
        public Inventory(Pawn pawn)
        {
            owner = pawn;
        }
        public void Add(Item item)
        {
            items.Add(item);
            item.inventory = this;
        }
        public List<Item> items = new List<Item>();
        public Pawn owner;
    }
}
