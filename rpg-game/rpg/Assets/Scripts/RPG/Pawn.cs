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
        bool CanMove
        {
            get
            {
                return Time.time >= nextMoveTime;
            }
        }
        public TileTransform TileTransform => GetComponent<TileTransform>();
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
        public new string name;
        public string partyName;
        public Party party;
        float moveDelay = 0.125f;  // Amount of time in seconds between movements
        float nextMoveTime = 0;
        public float maxHealth = 100.0f;
        public float health = 100.0f;
        public Items.Inventory inventory;
        public System.Action<DamageInfo> OnDamageTaken;
        private void Start()
        {
            inventory = new Items.Inventory(this);
            inventory.Add(Instantiate(Items.Weapon.GetRandomWeapon()));
            MoveToWalkableSpace();
            transform.position = World.WorldCoordToScene(TileTransform.coordinates);
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
            Vector3 targetGOPosition = (Vector3)(TileTransform.coordinates + Vector3.one / 2.0f) * World.tileSize;
            Vector3 position = Vector3.MoveTowards(transform.position, targetGOPosition, moveDelay);
            transform.position = position;
        }
        /// <summary>
        /// Attempts to move the pawn in the world
        /// </summary>
        /// <param name="displacement">Offset in tiles to displace the pawn by</param>
        public void Move(Vector3Int displacement)
        {
            if (CanMove == false)
                return;

            if (displacement == Vector3Int.zero)
                return;

            // Prevent diagonal movements
            //for (int i = 0; i < 3; i++)
            //{
            //    if (displacement[i] != 0)
            //    {
            //        // Get the other two indices/Components
            //        displacement[(i + 2) % 3] = 0;
            //        displacement[(i + 1) % 3] = 0;
            //        break;
            //    }
            //}

            Debug.DrawRay(transform.position, displacement, Color.yellow, moveDelay);
            Vector3Int targetCoordinates = TileTransform.coordinates + displacement;
            // Restrict to map bounds
            targetCoordinates.x = Mathf.Clamp(targetCoordinates.x, 0, World.instance.mapWidth - 1);
            targetCoordinates.y = Mathf.Clamp(targetCoordinates.y, 0, World.instance.mapHeight - 1);
            targetCoordinates.z = Mathf.Clamp(targetCoordinates.z, 0, World.instance.mapLength - 1);

            // To do: Give tiles flags (IE walkable, solid) instead
            // Test whether the coordinates are walkable
            if (CanStandHere(targetCoordinates))
            {
                this.transform.rotation = Quaternion.LookRotation(-displacement);
                TileTransform.coordinates = targetCoordinates;
                nextMoveTime = Time.time + moveDelay;
            }
        }
        /// <summary>
        /// Teleports the pawn to a walkable tile
        /// </summary>
        void MoveToWalkableSpace()
        {
            if (World.instance.MapReady == false)
                return;

            if (TileTools.GetClosestCoord(TileTransform.coordinates, delegate (Vector3Int c) { return CanStandHere(c); }, out Vector3Int coords))
                TileTransform.coordinates = coords;
        }
        public void TakeDamage(DamageInfo damage)
        {
            health -= damage.damage;
            OnDamageTaken?.Invoke(damage);
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