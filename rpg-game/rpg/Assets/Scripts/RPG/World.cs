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

        // https://learn.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files
        public static World Current
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<World>();
                }
                return instance;
            }
        }
        static World instance;  // Singleton pattern
        public Dictionary<Vector3Int, Chunk> chunks;
        public HashSet<ChunkLoader> loaders;
        private void Awake()
        {
            TileMaterial.BuildMaterials();
            TileShape.BuildShapes();

            if (RPG.Editor.Serializer.LoadWorldJSON("world", this) == false)
                chunks = new Dictionary<Vector3Int, Chunk>();
            if (loaders == null)
                loaders = new HashSet<ChunkLoader>();
        }
        private void OnDestroy()
        {
            RPG.Editor.Serializer.SaveWorldJSON("world", this);
            //RPG.Editor.Serializer.SaveWorld("world", this);
        }
        private void LateUpdate()
        {
            foreach (Chunk chunk in chunks.Values)
                UpdateChunk(chunk);
        }
        public void UpdateChunk(Chunk chunk)
        {
            //bool visible = false;
            //foreach (ChunkLoader loader in loaders)
            //{
            //    BoundsInt bounds = loader.Bounds;
            //    if (bounds.Contains(chunk.coords))
            //    {
            //        visible = true;
            //        break;
            //    }
            //}

            if (chunk.component != null)
            {
                if (chunk.component.dirtyMesh)
                {
                    chunk.component.dirtyMesh = false;
                    MeshGenerator.Generate(chunk);
                }
            }
        }
        public void AssertChunkLoader(Vector3Int coords, ChunkLoader loader)
        {
            if (chunks.TryGetValue(coords, out Chunk chunk))
            {

            }
            else
            {
                chunk = CreateChunk(coords);
                CreateChunkComponent(chunk);
            }

        }
        /// <summary>
        /// Returns the tile at world coordinates x, y, z
        /// </summary>
        /// <returns></returns>
        public Tile GetTile(Vector3Int worldCoords)
        {
            // Get the chunk coords
            Vector3Int chunkCoords = Chunk.WorldToChunk(worldCoords);
            if (chunks.TryGetValue(chunkCoords, out Chunk chunk))
            {
                return chunk.GetTile(worldCoords - chunkCoords * Chunk.size);
            }
            else
            {
                return Tile.Air;
            }
        }
        public void SetTile(Vector3Int worldCoords, Tile tile)
        {
            Vector3Int chunkCoords = Chunk.WorldToChunk(worldCoords);

            if (chunks.TryGetValue(chunkCoords, out Chunk chunk) == false)
            {
                chunk = CreateChunk(chunkCoords);
                CreateChunkComponent(chunk);
            }

            Vector3Int local = Chunk.WorldToLocal(worldCoords);
            chunk.tiles[Chunk.FlattenIndex(local.x, local.y, local.z)] = tile;
            chunk.DirtyMesh();

            void TryUpdate(Vector3Int coords)
            {
                if (World.Current.chunks.TryGetValue(coords, out Chunk neighbor))
                {
                    neighbor.DirtyMesh();
                }
            }

            if (local.x == 0)
            {
                TryUpdate(chunkCoords + Vector3Int.left);
            }
            else if (local.x == Chunk.size - 1)
            {
                TryUpdate(chunkCoords + Vector3Int.right);
            }
            if (local.y == 0)
            {
                TryUpdate(chunkCoords + Vector3Int.down);
            }
            else if (local.y == Chunk.size - 1)
            {
                TryUpdate(chunkCoords + Vector3Int.up);
            }
            if (local.z == 0)
            {
                TryUpdate(chunkCoords + Vector3Int.back);
            }
            else if (local.z == Chunk.size - 1)
            {
                TryUpdate(chunkCoords + Vector3Int.forward);
            }
        }
        public bool IsSolid(Vector3Int coords)
        {
            Tile tile = GetTile(coords);
            return tile.IsSolid;
        }
        public Chunk CreateChunk(Vector3Int chunkCoords)
        {
            Chunk chunk = new Chunk();
            chunk.coords = chunkCoords;
            chunk.tiles = new Tile[Chunk.sizeCubed];
            chunks.Add(chunkCoords, chunk);
            
            return chunk;
        }
        public ChunkComponent CreateChunkComponent(Chunk chunk)
        {
            GameObject chunkObject = new GameObject(chunk.coords.ToString());
            chunkObject.transform.parent = this.transform;
            ChunkComponent component = chunkObject.AddComponent<ChunkComponent>();
            component.gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Atlas");
            component.gameObject.AddComponent<MeshFilter>();
            component.gameObject.AddComponent<MeshCollider>();
            component.chunk = chunk;
            chunk.component = component;
            chunkObject.transform.position = chunk.coords * Chunk.size;

            MeshGenerator.Generate(chunk);
            return component;
        }
        public void SetChunks(Dictionary<Vector3Int, Chunk> chunks)
        {
            this.chunks = chunks;

            foreach (Chunk chunk in chunks.Values)
            {
                CreateChunkComponent(chunk);
            }
        }
        public TileTransform SpawnProp(string prefabName, Vector3Int coords, Quaternion rotation = default)
        {
            string directory = $"Prefabs/{prefabName}";
            GameObject prefab = Resources.Load<GameObject>(directory);
            if (prefab == null)
                return null;

            GameObject prop = Instantiate(prefab, coords, rotation);
            TileTransform tile = prop.GetComponent<TileTransform>();
            if (tile == null)
            {
                tile = prop.AddComponent<TileTransform>();
            }

            tile.coordinates = coords;

            return tile;
        }
    }
}