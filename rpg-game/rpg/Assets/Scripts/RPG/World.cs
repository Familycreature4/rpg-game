using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [UnityEngine.ExecuteInEditMode]
    public class World : MonoBehaviour, ISerializationCallbackReceiver
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
        public Dictionary<Vector3Int, Chunk> Chunks { get { return chunks; } }
        Chunk[] chunkCache;
        Dictionary<Vector3Int, Chunk> chunks;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            TileMaterial.BuildMaterials();
            TileShape.BuildShapes();

            if (chunks == null)
            {
                chunks = new Dictionary<Vector3Int, Chunk>();
            }
        }
        private void OnDestroy()
        {
            ClearChunks();
        }
        private void Update()
        {
            if (chunks == null)
                return;

            foreach (Chunk chunk in chunks.Values)
            {
                if (chunk != null)
                    UpdateChunk(chunk);
            }
        }
        void UpdateChunk(Chunk chunk)
        {
            if (chunk.IsDirty)
            {
                // Get (or create) a gameobject representation of this chunk
                if (chunk.component == null)
                {
                    CreateChunkComponent(chunk);
                }

                MeshGenerator.Generate(chunk);
                chunk.IsDirty = false;
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
            }

            Vector3Int local = Chunk.WorldToLocal(worldCoords);
            chunk.tiles[Chunk.FlattenIndex(local.x, local.y, local.z)] = tile;
            chunk.IsDirty = true;

            void TryUpdate(Vector3Int coords)
            {
                if (World.Current.chunks.TryGetValue(coords, out Chunk neighbor))
                {
                    neighbor.IsDirty = true;
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
        Chunk CreateChunk(Vector3Int chunkCoords)
        {
            Chunk chunk = new Chunk();
            chunk.coords = chunkCoords;
            chunk.tiles = new Tile[Chunk.sizeCubed];
            chunks.Add(chunkCoords, chunk);
            
            return chunk;
        }
        public void DirtyChunks()
        {
            foreach (Chunk chunk in chunks.Values)
            {
                chunk.IsDirty = true;
            }
        }
        ChunkComponent CreateChunkComponent(Chunk chunk)
        {
            GameObject chunkObject = new GameObject(chunk.coords.ToString());
            chunkObject.hideFlags = HideFlags.HideAndDontSave;
            chunkObject.transform.parent = this.transform;
            ChunkComponent component = chunkObject.AddComponent<ChunkComponent>();
            component.gameObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Atlas");
            component.gameObject.AddComponent<MeshFilter>();
            component.gameObject.AddComponent<MeshCollider>();
            component.chunk = chunk;
            chunk.component = component;
            chunkObject.transform.position = chunk.coords * Chunk.size;
            return component;
        }
        public void SetChunks(Dictionary<Vector3Int, Chunk> chunks)
        {
            // Clear existing chunks
            ClearChunks();

            this.chunks = chunks;

            foreach (Chunk chunk in chunks.Values)
            {
                CreateChunkComponent(chunk);
                chunk.IsDirty = true;
            }
        }
        public void ClearChunks()
        {
            for (int c = transform.childCount - 1; c >= 0; c--)
            {
                Transform child = transform.GetChild(c);

                if (child.gameObject.GetComponent<ChunkComponent>() != null)
                {
                    if (Application.isEditor)
                    {
                        GameObject.DestroyImmediate(child.gameObject);
                    }
                    else
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                }
            }

            if (chunks != null)
                chunks.Clear();
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

        public void OnBeforeSerialize()
        {
            if (chunks == null)
                return;

            chunkCache = new Chunk[chunks.Count];
            int index = 0;
            foreach (Chunk chunk in chunks.Values)
            {
                chunkCache[index] = chunk;
                index++;
            }
        }

        public void OnAfterDeserialize()
        {
            if (chunkCache != null)
            {
                if (chunks == null)
                {
                    chunks = new Dictionary<Vector3Int, Chunk>();
                }

                foreach (Chunk chunk in chunkCache)
                {
                    chunks.Add(chunk.coords, chunk);
                }
            }
        }
    }
}