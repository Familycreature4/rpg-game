using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Items;
using RPG.Orders;
namespace RPG
{
    /// <summary>
    /// A character which exists in the world
    /// </summary>
    public class Pawn : MonoBehaviour
    {
        // THIS HAS BEEN CERTIFIED BY THE CREATURE
        // CREATURE CODE QUALITY STAMP OF APPROVAL
        public bool CanMove
        {
            get
            {
                return Time.time >= nextMoveTime;
            }
        }
        public TileTransform TileTransform => GetComponent<TileTransform>();
        public List<Order> Orders => orders;
        public int Health
        {
            get
            {
                return health;
            }
        }
        public bool IsAlive => Health > 0;
        public bool IsLeader
        {
            get
            {
                if (party != null)
                {
                    return this == party.Leader;
                }
                else
                {
                    return true;
                }
            }
        }
        public Vector3Int Coordinates
        {
            get
            {
                return TileTransform.coordinates;
            }
            set
            {
                TileTransform.coordinates = value;
            }
        }
        List<Order> orders = new List<Order>();
        public Party party;
        public Items.Inventory inventory;
        public Stats stats;
        public Sprite image;
        public new string name;
        float moveDelay = 0.2f;  // Amount of time in seconds between movements
        float nextMoveTime = 0;
        public float battleMoveDistance = 5.0f;
        int health;
        private void Start()
        {
            inventory = new Items.Inventory(this);
            inventory.Add(Instantiate(Items.Weapon.GetRandomWeapon()));
            //MoveToWalkableSpace();
            transform.position = TileTransform.coordinates + Vector3.one / 2.0f;

            orders.Add(new Orders.MoveTo(this, () => {
                if (party != null && party.Pawns != null)
                {
                    int index = party.IndexOf(this);
                    if (index < party.formationLocalPositions.Length && index >= 0)
                        return party.TransformFormationPosition(party.formationLocalPositions[index]);
                }

                return this.Coordinates;
            }, false));

            health = 100;
        }
        private void Update()
        {
            // Get first order
            if (orders.Count > 0 && (IsLeader == false || Director.Current.battleManager != null))
            {
                // Sort orders
                orders.Sort(delegate (Order orderA, Order orderB) { return orderA.Priority.CompareTo(orderB.Priority); });
                Order order = orders[0];
                if (order.IsComplete)
                {
                    orders.RemoveAt(0);
                }
                else
                {
                    order.Think();
                }
            }
        }
        private void LateUpdate()
        {
            // Move the gameobject to the world coordinates
            Vector3 targetGOPosition = (Vector3)(TileTransform.coordinates + Vector3.one / 2.0f);
            Vector3 position = Vector3.MoveTowards(transform.position, targetGOPosition, moveDelay);
            transform.position = position;
        }
        /// <summary>
        /// Attempts to move the pawn in the world
        /// </summary>
        /// <param name="displacement">Offset in tiles to displace the pawn by</param>
        public bool Move(Vector3Int displacement)
        {
            if (CanMove == false)
                return false;

            if (displacement == Vector3Int.zero)
                return false;

            Vector3Int targetCoordinates = TileTransform.coordinates + displacement;

            // Test whether the coordinates are walkable
            if (MoveHelper.TryMove(TileTransform.coordinates, targetCoordinates, TileTransform.size, out Vector3Int newCoords, TileTransform, party))
            {
                Vector3Int realDisplacement = newCoords - TileTransform.coordinates;
                if (realDisplacement != Vector3Int.zero)
                {
                    Vector3 lookDirection = -realDisplacement;
                    lookDirection.y = 0;
                    if (lookDirection.sqrMagnitude > 0)
                        this.transform.rotation = Quaternion.LookRotation(lookDirection);
                    TileTransform.coordinates = newCoords;
                    nextMoveTime = Time.time + moveDelay;

                    return true;
                }
                
            }

            return false;
        }
        public void ResetMoveTimer()
        {
            nextMoveTime = Time.time;
        }
        public bool CanStandHere(Vector3Int coord) => MoveHelper.CanStandHere(coord, TileTransform.size, TileTransform, party);
        public void Damage(int amount)
        {
            int newHealth = Mathf.Max(health - amount, 0);
            health = newHealth;

            if (party == RPGPlayer.Current.Party && health <= 0)
                health = 1;

            EventManager.onPawnDamaged?.Invoke(this, amount);

            if (health <= 0)
            {
                OnDeath();
            }
        }
        void OnDeath()
        {
            gameObject.SetActive(false);
            //onPawnDeath?.Invoke(this);
        }
    }
}