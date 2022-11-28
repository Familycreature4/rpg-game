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
                return Time.time >= nextMoveTime && (Director.Current != null && Director.Current.battleManager == null);
            }
        }
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
        public new string name;
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
        private void Start()
        {
            stats = new Stats();
            inventory = new Items.Inventory(this);
            inventory.Add(Instantiate(Items.Weapon.GetRandomWeapon()));
            //MoveToWalkableSpace();
            transform.position = TileTransform.coordinates + Vector3.one / 2.0f;
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
        public bool CanStandHere(Vector3Int coord) => MoveHelper.CanStandHere(coord, TileTransform.size, TileTransform, party);
    }
}