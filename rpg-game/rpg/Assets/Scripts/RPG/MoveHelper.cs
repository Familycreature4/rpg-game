using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace RPG
{
    public class MoveHelper
    {
        public static bool CanStandHere(Vector3Int position, Vector3Int size, TileTransform myTransform = null, Party myParty = null)
        {
            // Check if the coordinate below is solid 

            if (World.Current.IsSolid(position + Vector3Int.down) == false)
                return false;

            // Overlap volume on environment
            foreach (Vector3Int c in new Bounds(position, size).AllCoords())
            {
                if (World.Current.IsSolid(c))
                    return false;

                if (TileTools.TryGetTileTransform(c, out TileTransform t))  // Replace by comparing min/max
                {
                    if (myTransform != null && t == myTransform)
                        continue;

                    if (myParty != null && t.gameObject.TryGetComponent<Pawn>(out Pawn pawn) && pawn.party == myParty)
                        continue;

                    return false;
                }
            }

            return true;
        }
        public static bool TryMove(Vector3Int from, Vector3Int to, Vector3Int size, out Vector3Int newCoords, TileTransform myTransform = null, Party myParty = null)
        {
            newCoords = to;
            if (CanStandHere(to, size, myTransform, myParty))
            {
                return true;
            }
            else
            {
                Tile targetCoordinatesTile = World.Current.GetTile(to);
                if (targetCoordinatesTile.IsSolid)
                {
                    if (targetCoordinatesTile.shape != null && targetCoordinatesTile.shape.name == "Stairs")
                    {
                        // Try to move up
                        if (CanStandHere(to + Vector3Int.up, size, myTransform, myParty))
                        {
                            newCoords = to + Vector3Int.up;
                            return true;
                        }
                    }
                }
                else  // Is air. Check if FROM is below a stair tile to move down
                {
                    Tile stairTile = World.Current.GetTile(from + Vector3Int.down);
                    if (stairTile.shape != null && stairTile.shape.name == "Stairs")
                    {
                        if (CanStandHere(to - Vector3Int.up, size, myTransform, myParty))
                        {
                            newCoords = to - Vector3Int.up;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
