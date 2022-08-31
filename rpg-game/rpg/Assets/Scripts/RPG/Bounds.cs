using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public struct Bounds
    {
        public Bounds(TileTransform t)
        {
            mins = new Vector3Int(
                t.coordinates.x - Mathf.RoundToInt(t.size.x / 2),
                t.coordinates.y,
                t.coordinates.z - Mathf.RoundToInt(t.size.z / 2)
            );
            size = t.size;
        }
        public Bounds(Vector3Int coords, Vector3Int size)
        {
            mins = new Vector3Int(
                coords.x - Mathf.RoundToInt(size.x / 2),
                coords.y,
                coords.z - Mathf.RoundToInt(size.z / 2)
            );
            this.size = size;
        }
        public Vector3Int Maxs => mins + size - Vector3Int.one;
        public Vector3 Center => (Vector3)(Maxs + mins) / 2.0f;
        public Vector3Int mins;
        public Vector3Int size;
        /// <summary>
        /// Returns whether this bounds overlaps the other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Overlaps(Bounds other)
        {
            Vector3Int myMax = Maxs;
            Vector3Int otherMax = other.Maxs;

            if (myMax.x < other.mins.x)
                return false;

            if (myMax.y < other.mins.y)
                return false;

            if (myMax.z < other.mins.z)
                return false;

            if (mins.x > otherMax.x)
                return false;

            if (mins.y > otherMax.y)
                return false;

            if (mins.z > otherMax.z)
                return false;

            return true;
        }
        public bool Contains(Vector3Int c)
        {
            Vector3Int maxs = Maxs;
            return c.x >= mins.x && c.x <= maxs.x && c.y >= mins.y && c.y <= maxs.y && c.z >= mins.z && c.z <= maxs.z;
        }
        public IEnumerable<Vector3Int> AllCoords()
        {
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        yield return mins + new Vector3Int(x, y, z);
                    }
                }
            }
        }
    }
}
