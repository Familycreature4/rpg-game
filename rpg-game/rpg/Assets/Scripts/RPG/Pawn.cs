using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Items;
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
        public bool IsDead => health <= 0;
        public TileTransform TileTransform => GetComponent<TileTransform>();
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
        public Party party;
        public Items.Inventory inventory;
        public Stats stats;
        public System.Action<DamageInfo> OnDamageTaken;
        public System.Action onDeath;
        public new string name;
        public string partyName;
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
        float moveDelay = 0.2f;  // Amount of time in seconds between movements
        float nextMoveTime = 0;
        public float maxHealth = 100.0f;
        public float health = 100.0f;
        private void Start()
        {
            stats = new Stats();
            inventory = new Items.Inventory(this);
            inventory.Add(Instantiate(Items.Weapon.GetRandomWeapon()));
            //MoveToWalkableSpace();
            transform.position = TileTransform.coordinates + Vector3.one / 2.0f;
            Party.AddToParty(partyName, this);
            health = maxHealth;
        }
        private void Update()
        {
            if (party != null)
            {
                int index = party.pawns.IndexOf(this);
                Vector3Int targetPosition = party.TransformFormationPosition(party.formationLocalPositions[index]);
                bool inFormation = Coordinates == targetPosition;
                bool isLeader = index == 0;

                // Leader is manually controlled
                // Have non-leader pawns path to formation position    
                if (isLeader == false && inFormation == false)
                {
                    List<Vector3Int> path = new List<Vector3Int>();
                    if (TileTools.GetPath(Coordinates, targetPosition, delegate(Vector3Int c) { return this.CanStandHere(c); }, out path, (int)Vector3.Distance(Coordinates, targetPosition) * 10 + 10))
                    {
                        Vector3Int targetPos = path[0];
                        if (path.Count > 1)
                            targetPos = path[1];
                        Move(targetPos - Coordinates);

                        for (int a = 0; a < path.Count - 1; a++)
                        {
                            Debug.DrawLine(path[a] + Vector3.one / 2, path[a + 1] + Vector3.one / 2, Color.white);
                        }
                        for (int a = 0; a < path.Count; a++)
                        {
                            Debug.DrawRay(path[a] + Vector3.one / 2, Vector3.up, Color.red);
                        }
                    }
                }

                if (isLeader || inFormation)
                {
                    this.transform.rotation = Quaternion.AngleAxis(party.FormationRotation + 180.0f, Vector3.up);
                }
            }
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

            Debug.DrawRay(transform.position, displacement, Color.yellow, moveDelay);
            Vector3Int targetCoordinates = TileTransform.coordinates + displacement;

            // To do: Give tiles flags (IE walkable, solid) instead
            // Test whether the coordinates are walkable
            if (CanStandHere(targetCoordinates))
            {
                this.transform.rotation = Quaternion.LookRotation(-displacement);
                TileTransform.coordinates = targetCoordinates;
                nextMoveTime = Time.time + moveDelay;

                return true;
            }

            return false;
        }
        /// <summary>
        /// Teleports the pawn to a walkable tile
        /// </summary>
        void MoveToWalkableSpace()
        {
            if (TileTools.GetClosestCoord(TileTransform.coordinates, delegate (Vector3Int c) { return CanStandHere(c); }, out Vector3Int coords))
                TileTransform.coordinates = coords;
        }
        public void ResetMoveTime()
        {
            nextMoveTime = Time.time;
        }
        public void InvokeMoveDelay()
        {
            nextMoveTime = Time.time + moveDelay;
        }
        public void TakeDamage(DamageInfo damage)
        {
            health = Mathf.Max(health - damage.damage, 0);
            OnDamageTaken?.Invoke(damage);

            if (health <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        public bool CanStandHere(Vector3Int coord)
        {
            if (World.Current.IsSolid(coord + Vector3Int.down) == false)
                return false;

            foreach (Vector3Int c in new Bounds(coord, TileTransform.size).AllCoords())
            {
                if (World.Current.IsSolid(c))
                    return false;

                if (TileTools.TryGetTileTransform(c, out TileTransform t) && t != TileTransform)
                {
                    if (t.TryGetComponent<Pawn>(out Pawn p))
                    {
                        if (party != null && party.Contains(p) == false)
                            return false;
                    }
                    else
                    {
                        return false;
                    }

                }
            }

            return true;
        }
        public Items.Weapon GetWeapon()
        {
            foreach (Items.Item item in inventory.items)
            {
                if (item is Weapon weapon)
                    return weapon;
            }

            return null;
        }
    }
}