using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public class World : MonoBehaviour
    {
        // The 'static' modifier will assign the given property/field/method to the class TYPE instead of a given instance of the class
        // Any property/field/method which is static AND public is accessible from any part of the game (IE global access)
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/static
        public static World Current => instance;
        public static World instance;  // Singleton pattern
        public static float tileSize = 1.0f;  // Size of a tile in the scene
        public Tile[] tiles;
        public bool MapReady => tiles != null;
        public int mapVolume => mapWidth * mapLength * mapHeight;
        public int mapWidth = 16;
        public int mapLength = 16;
        public int mapHeight = 2;
        public string mapName = "mapv2";

        private void Awake()
        {
            if (instance == null)
                instance = this;

            TileShape.BuildShapes();
            TileMaterial.BuildMaterials();

            WorldGenerator.GenerateMap(this);
            MeshGenerator.Generate(this);
        }
        /// <summary>
        /// Returns the tile at coordinates x, y, z
        /// </summary>
        /// <returns></returns>
        public Tile GetTile(int x, int y, int z)
        {
            // If coords are beyond range, return air
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight || z < 0 || z >= mapLength)
                return Tile.Air;

            return tiles[FlattenIndex(x, y, z)];
        }
        public Tile GetTile(Vector3Int c) => GetTile(c.x, c.y, c.z);
        public Tile GetTile(int index) => tiles[index];
        public bool InBounds(Vector3Int coords)
        {
            return coords.x >= 0 && coords.x < mapWidth && coords.y >= 0 && coords.y < mapHeight && coords.z >= 0 && coords.z < mapLength;
        }
        public bool IsSolid(Vector3Int coords)
        {
            Tile tile = GetTile(coords);
            return tile.IsSolid;
        }
        public Vector3Int UnFlattenIndex(int index)
        {
            int y = index / (mapWidth * mapLength);
            index -= (y * (mapWidth * mapLength));
            int z = index / mapWidth;
            int x = index % mapWidth;
            return new Vector3Int(x, y, z);
        }
        public int FlattenIndex(int x, int y, int z)
        {
            return (mapWidth * mapLength) * y + z * mapWidth + x;
        }
        public static Vector3 WorldCoordToScene(Vector3 world)
        {
            return (world) * World.tileSize;
        }
        public static Vector3Int SceneToWorldCoords(Vector3 pos)
        {
            pos /= World.tileSize;
            return Vector3Int.FloorToInt(pos);
        }
        private void OnDrawGizmosSelected()
        {
            if (MapReady == false)
                return;

            for (int i = 0; i < mapVolume; i++)
            {
                Tile tile = GetTile(i);
                if (tile.IsSolid)
                {
                    Vector3Int coords = UnFlattenIndex(i);
                    Gizmos.color = coords.y == 0 ? Color.black : Color.white;
                    Gizmos.DrawWireCube(coords + (Vector3.one / 2) * tileSize, Vector3.one * tileSize);
                }
            }
        }
    }
}