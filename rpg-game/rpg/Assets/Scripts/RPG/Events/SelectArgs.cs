using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG;
namespace RPG.Events
{
    public class SelectArgs : EventArgs
    {
        public SelectArgs(Selector selector, RaycastHit hit)
        {
            this.selector = selector;

            SetRaycastHit(hit);
        }
        public SelectArgs(Pawn pawn)
        {
            this.pawn = pawn;
            world = false;
        }
        public bool IsWorld => world;
        public bool IsUI => world == false;
        public bool IsPawn => Pawn != null;
        public bool Consumed => consumed;
        public GameObject GameObject => raycastHit.collider == null ? null : raycastHit.collider.gameObject;
        public Pawn Pawn
        {
            get
            {
                if (this.pawn != null)
                    return this.pawn;

                GameObject gameObject = this.GameObject;
                if (gameObject != null && gameObject.TryGetComponent(out Pawn pawn))
                    return pawn;

                return null;
            }
        }
        public Selector selector;
        public RaycastHit raycastHit;
        bool world;
        bool consumed;
        Pawn pawn;
        public void Use()
        {
            consumed = true;
        }
        public void SetRaycastHit(RaycastHit hit)
        {
            pawn = null;
            world = true;

            raycastHit = hit;
            consumed = false;
        }
    }
}
