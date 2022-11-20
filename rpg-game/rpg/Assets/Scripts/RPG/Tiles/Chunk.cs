using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [System.Serializable]
    public class Chunk
    {
        public const int size = 16;
        public const int sizeSquared = size * size;
        public const int sizeCubed = size * size * size;

        public bool IsDirty { set; get; }
        public Tile[] tiles;
        public Vector3Int coords;
        public ChunkComponent component;
        public static Vector3Int UnFlattenIndex(int index)
        {
            int y = index / (size * size);
            index -= (y * (size * size));
            int z = index / size;
            int x = index % size;
            return new Vector3Int(x, y, z);
        }
        public static int FlattenIndex(int x, int y, int z)
        {
            return (size * size) * y + z * size + x;
        }
        public static Vector3Int WorldToLocal(Vector3 world)
        {
            return Vector3Int.FloorToInt(world - WorldToChunk(world) * Chunk.size);
        }
        public static Vector3Int WorldToChunk(Vector3 world)
        {
            return Vector3Int.FloorToInt(world / size);
        }
        public Tile GetTile(int x, int y, int z)
        {
            return tiles[FlattenIndex(x, y, z)];
        }
        public Tile GetTile(Vector3Int c) => GetTile(c.x, c.y, c.z);
        public Tile GetTile(int index) => tiles[index];
        public bool InBounds(Vector3Int coords)
        {
            return coords.x >= 0 && coords.x < size && coords.y >= 0 && coords.y < size && coords.z >= 0 && coords.z < size;
        }
    }
}
