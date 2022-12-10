using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public static class TileTools
    {
        [System.Flags]
        public enum CastFilter
        {
            World = 1 << 0,
            Objects = 1 << 1
        }
        public readonly static Vector3Int[] directionsAll = new Vector3Int[]
        {
        new Vector3Int(-1, 1, 1), new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1),
        new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0),
        new Vector3Int(-1, 1, -1), new Vector3Int(0, 1, -1), new Vector3Int(1, 1, -1),

        new Vector3Int(-1, 0, 1), new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1),
        new Vector3Int(-1, 0, 0),                            new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, -1), new Vector3Int(0, 0, -1), new Vector3Int(1, 0, -1),

        new Vector3Int(-1, -1, 1), new Vector3Int(0, -1, 1), new Vector3Int(1, -1, 1),
        new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, -1), new Vector3Int(0, -1, -1), new Vector3Int(1, -1, -1),
        };
        public static bool GetPath( Vector3Int start,
                                    Vector3Int end, 
                                    out List<Vector3Int> path, 
                                    System.Func<Vector3Int, bool> coordCheck = null, 
                                    System.Func<Vector3Int, Vector3Int, bool> moveToCheck = null,
                                    int maxIterations = -1)
        {
            int GetHCost(Vector3Int start, Vector3Int end)
            {
                return Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y) + Mathf.Abs(end.z - start.z);
            }

            path = null;

            if (start == end)
            {
                path = new List<Vector3Int> { end };
                return true;
            }

            List<ATile> openTiles = new List<ATile>();
            HashSet<Vector3Int> closedTiles = new HashSet<Vector3Int>();
            Dictionary<Vector3Int, ATile> tiles = new Dictionary<Vector3Int, ATile>();
            if (coordCheck == null)
            {
                coordCheck = delegate (Vector3Int coord)
                {
                    return World.Current.IsSolid(coord) == false && World.Current.IsSolid(coord + Vector3Int.down);
                };
            }

            openTiles.Add(new ATile { gCost = 0, coords = start, hCost = GetHCost(start, end) });
            tiles.Add(start, openTiles[0]);

            if (maxIterations == -1)
                maxIterations = 100 * GetHCost(start, end);
            int iteration = 0;

            while (openTiles.Count > 0 && iteration < maxIterations)
            {
                iteration++;
                // Sort open tiles based on FCost 
                openTiles.Sort(delegate (ATile a, ATile b) { return a.FCost.CompareTo(b.FCost); });

                // Pop lowest fCost tile
                ATile currentTile = openTiles[0];
                openTiles.RemoveAt(0);
                closedTiles.Add(currentTile.coords);

                foreach (Vector3Int direction in directionsAll)
                {
                    Vector3Int neighborCoords = currentTile.coords + direction;

                    if (moveToCheck != null && moveToCheck(currentTile.coords, neighborCoords) == false)
                        continue;

                    if (neighborCoords == end)
                    {
                        // PATH FOUND
                        if (path == null)
                            path = new List<Vector3Int>();

                        path.Add(neighborCoords);
                        path.Add(currentTile.coords);
                        ATile parent = currentTile.parent;
                        while (parent != null)
                        {
                            path.Add(parent.coords);
                            parent = parent.parent;
                        }

                        path.Reverse();
                        return true;
                    }

                    Tile neighborWorldTile = World.Current.GetTile(neighborCoords);
                    if (closedTiles.Contains(neighborCoords) || coordCheck(neighborCoords) == false)
                        continue;

                    // Calculate new gCost
                    int neighborGCost = currentTile.gCost + GetHCost(currentTile.coords, neighborCoords);

                    // Get neighbor tile
                    ATile neighborTile;
                    if (tiles.TryGetValue(neighborCoords, out neighborTile) == false)
                    {
                        // Create ATile
                        neighborTile = new ATile { gCost = neighborGCost, coords = neighborCoords, hCost = GetHCost(neighborCoords, end) };
                        openTiles.Add(neighborTile);
                        tiles.Add(neighborCoords, neighborTile);
                        neighborTile.parent = currentTile;
                    }

                    if (neighborGCost < neighborTile.gCost)
                    {
                        neighborTile.parent = currentTile;
                        neighborTile.gCost = neighborGCost;
                    }
                }
            }

            path = null;
            return false;
        }
        public static bool GetClosestCoord(Vector3Int start, System.Func<Vector3Int, bool> function, out Vector3Int coords)
        {
            if (function == null)
                function = delegate (Vector3Int c) { return IsWalkable(c, new Vector3Int(1, 2, 1)); };

            HashSet<Vector3Int> explored = new HashSet<Vector3Int>();
            explored.Add(start);
            List<Vector3Int> openCoords = new List<Vector3Int>();

            openCoords.Add(start);
            int maxIterations = 1000;
            int iteration = 0;

            while (openCoords.Count > 0 && iteration < maxIterations)
            {
                iteration++;
                Vector3Int currentCoords = openCoords[0];
                openCoords.RemoveAt(0);

                if (function(currentCoords))
                {
                    coords = currentCoords;
                    return true;
                }

                foreach (Vector3Int direction in directionsAll)
                {
                    Vector3Int neighborCoords = direction + currentCoords;
                    if (explored.Contains(neighborCoords) == false)
                    {
                        openCoords.Add(neighborCoords);
                        explored.Add(neighborCoords);
                    }
                }
            }

            coords = start;
            return false;
        }
        public class ATile
        {
            public ATile parent;
            public int FCost => gCost + hCost;
            public int gCost;
            public int hCost;
            public Vector3Int coords;
        }
        public static bool Overlap(Vector3Int coords, Vector3Int size, CastFilter filter = CastFilter.World | CastFilter.Objects, TileTransform myTransform = null)
        {
            RPG.Bounds bounds = new Bounds(coords, size);

            if (filter.HasFlag(CastFilter.World))
            {
                foreach (Vector3Int c in bounds.AllCoords())
                {
                    if (World.Current.IsSolid(c))
                        return false;
                }
            }
            
            if (filter.HasFlag(CastFilter.Objects))
            {
                // Check whether any tile transforms are blocking this
                foreach (TileTransform t in TileTransform.transforms)
                {
                    if (t == myTransform || t.isActiveAndEnabled == false)
                        continue;

                    if (t.Bounds.Overlaps(bounds))
                        return false;
                }
            }

            return true;
        }
        public static bool IsWalkable(Vector3Int coord, Vector3Int size, CastFilter filter = CastFilter.World)
        {
            return Overlap(coord, size, filter) && World.Current.IsSolid(coord + Vector3Int.down);
        }
        public static bool TryGetTileTransform(Vector3Int coords, out TileTransform t)
        {
            foreach (TileTransform tileTransform in TileTransform.transforms)
            {
                if (tileTransform.isActiveAndEnabled == false)
                    continue;
                if (tileTransform.Bounds.Contains(coords))
                {
                    t = tileTransform;
                    return true;
                }
            }

            t = null;
            return false;
        }
        /// <summary>
        /// DDA cast in the world
        /// </summary>
        /// <param name="start">World Coordinates (NOT SCENE)</param>
        /// <param name="end">World Coordinates (NOT SCENE)</param>
        /// <param name="hits"></param>
        /// <returns></returns>
        public static bool Raycast(Vector3 start, Vector3 end, List<TileHit> hits, CastFilter filter = CastFilter.World | CastFilter.Objects)
        {
            int GetSmallestComponentIndex(Vector3 v)
            {
                float smallestDistance = Mathf.Infinity;
                int c = 0;
                for (int i = 0; i < 3; i++)
                {
                    float componentDistance = v[i];
                    if (componentDistance < smallestDistance)
                    {
                        smallestDistance = componentDistance;
                        c = i;
                    }
                }
                return c;
            }
            Vector3 difference = end - start;
            float maxDistance = difference.magnitude;
            float distance = 0;
            // Direction to step towards for each axis
            Vector3Int moveDirection = new Vector3Int((int)Mathf.Sign(difference.x), (int)Mathf.Sign(difference.y), (int)Mathf.Sign(difference.z));

            // How far the ray travels when moving 1 unit in each axis
            Vector3 stepSize = new Vector3
            {
                x = Mathf.Sqrt(1 + Mathf.Pow(difference.y / difference.x, 2) + Mathf.Pow(difference.z / difference.x, 2)),
                y = Mathf.Sqrt(1 + Mathf.Pow(difference.x / difference.y, 2) + Mathf.Pow(difference.z / difference.y, 2)),
                z = Mathf.Sqrt(1 + Mathf.Pow(difference.y / difference.z, 2) + Mathf.Pow(difference.x / difference.z, 2))
            };

            // Projected distance of each stepped axis thus far
            Vector3 projectedLength = new Vector3();

            Vector3Int endCoords = Vector3Int.FloorToInt(end);
            Vector3Int currentCoords = Vector3Int.FloorToInt(start);
            bool startSolid = World.Current.IsSolid(currentCoords);
            bool endSolid = World.Current.IsSolid(endCoords);

            // Set up initial projectedLength settings
            for (int i = 0; i < 3; i++)
            {
                projectedLength[i] = moveDirection[i] < 0 ? (start[i] - currentCoords[i]) * stepSize[i] : (currentCoords[i] + 1 - start[i]) * stepSize[i];
            }

            while (currentCoords != endCoords && distance < maxDistance)
            {
                // Walk along shortest projected path thus far
                // Get the index of the smallest component in the projectedlength vector
                int c = GetSmallestComponentIndex(projectedLength);

                currentCoords[c] += moveDirection[c];
                distance = projectedLength[c];
                projectedLength[c] += stepSize[c];

                // Check if current tile's solidity changes OR enters/leaves a Tile Transform
                // Check if current block's solidity is different than the previous one
                Tile tile = World.Current.GetTile(currentCoords);
                TryGetTileTransform(currentCoords, out TileTransform tileTransform);

                if ((hits.Count == 0 && (tile.IsSolid || tileTransform != null)) || ((hits[hits.Count - 1].tile.IsSolid) != (tile.IsSolid == false)) || (hits[hits.Count - 1].transform != tileTransform))
                {
                    hits.Add(new TileHit
                    {
                        didHit = true,
                        tile = tile,
                        tileCoords = currentCoords,
                        distance = distance,
                        end = end,
                        start = start,
                        hitPoint = start + difference.normalized * distance,
                        startSolid = startSolid,
                        endSolid = endSolid
                    });
                }
            }

            return hits.Count > 0;
        }
        public struct TileHit
        {
            public Vector3 start;
            public Vector3 end;
            public bool didHit;
            public float distance;
            public Vector3 hitPoint;
            public Vector3Int tileCoords;
            public Tile tile;
            public bool startSolid;
            public bool endSolid;
            public TileTransform transform;
        }
    }
}