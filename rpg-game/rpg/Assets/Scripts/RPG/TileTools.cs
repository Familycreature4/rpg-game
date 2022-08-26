using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileTools
{
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
    public static bool GetPath(Vector3Int start, Vector3Int end, System.Func<Vector3Int, bool> walkableFunction, out List<Vector3Int> path)
    {
        int GetHCost(Vector3Int start, Vector3Int end)
        {
            return Mathf.Abs(end.x - start.x) + Mathf.Abs(end.y - start.y) + Mathf.Abs(end.z - start.z);
        }

        path = null;
        List<ATile> openTiles = new List<ATile>();
        HashSet<Vector3Int> closedTiles = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, ATile> tiles = new Dictionary<Vector3Int, ATile>();
        if (walkableFunction == null)
        {
            walkableFunction = delegate (Vector3Int coord) { 
                return World.instance.GetTile(coord).IsSolid == false && World.instance.GetTile(coord + Vector3Int.down).IsSolid; 
            };
        }

        openTiles.Add(new ATile { gCost = 0, coords = start, hCost = GetHCost(start, end) });
        tiles.Add(start, openTiles[0]);

        int maxIterations = 100 * GetHCost(start, end);
        int iteration = 0;

        while(openTiles.Count > 0 && iteration < maxIterations)
        {
            iteration++;
            // Sort open tiles based on FCost 
            openTiles.Sort(delegate (ATile a, ATile b) { return a.FCost.CompareTo(b.FCost); });

            // Pop lowest fCost tile
            ATile currentTile = openTiles[0];
            openTiles.RemoveAt(0);
            closedTiles.Add(currentTile.coords);

            foreach (Vector3Int direction in Tile.neighbors)
            {
                Vector3Int neighborCoords = currentTile.coords + direction;

                if (neighborCoords == end)
                {
                    // PATH FOUND
                    if (path == null)
                        path = new List<Vector3Int>();
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

                Tile neighborWorldTile = World.instance.GetTile(neighborCoords);
                if (closedTiles.Contains(neighborCoords) || walkableFunction(neighborCoords) == false)
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
        int maxIterations = 1000000;
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
                if (World.instance.InBounds(neighborCoords) && explored.Contains(neighborCoords) == false)
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
    public static bool Overlap(Vector3Int coords, Vector3Int size, TileTransform myTransform = null)
    {
        BoundsInt bounds = GetBounds(coords, size);

        foreach (Vector3Int c in bounds.allPositionsWithin)
        {
            if (World.instance.GetTile(c).IsSolid)
                return false;
        }

        // Check whether any tile transforms are blocking this
        foreach (TileTransform t in TileTransform.transforms)
        {
            if (t == myTransform)
                continue;

            if (Overlaps(t.Bounds, bounds))
                return false;
        }

        return true;
    }
    public static bool IsWalkable(Vector3Int coord, Vector3Int size)
    {
        return Overlap(coord, size) && World.instance.GetTile(coord + Vector3Int.down).IsSolid;
    }
    public static BoundsInt GetBounds(Vector3Int coords, Vector3Int size) => new BoundsInt(coords.x, coords.y, coords.z, size.x, size.y, size.z);
    public static bool Overlaps(BoundsInt a, BoundsInt b)
    {
        if (a.max.x <= b.min.x)
            return false;

        if (a.max.y <= b.min.y)
            return false;

        if (a.max.z <= b.min.z)
            return false;

        if (a.min.x >= b.max.x)
            return false;

        if (a.min.x >= b.max.x)
            return false;

        if (a.min.y >= b.max.y)
            return false;

        if (a.min.z >= b.max.z)
            return false;

        return true;
    }
}
