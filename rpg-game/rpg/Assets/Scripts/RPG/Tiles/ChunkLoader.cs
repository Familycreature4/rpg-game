using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [UnityEngine.ExecuteInEditMode]
    public class ChunkLoader : MonoBehaviour
    {
        public BoundsInt Bounds
        {
            get
            {
                Vector3Int c = ChunkCoords;
                return new BoundsInt
                {
                    min = new Vector3Int(c.x - size.x / 2, c.y - size.y / 2, c.z - size.z / 2),
                    max = new Vector3Int(c.x + size.x / 2, c.y + size.y / 2, c.z + size.z / 2)
                };
            }
        }
        public Vector3Int ChunkCoords => Chunk.WorldToChunk(transform.position);
        Vector3Int lastChunkCoords;
        public Vector3Int size = new Vector3Int(2, 2, 2);
        private void Start()
        {
            if (World.instance != null)
                World.instance.loaders.Add(this);

            Assert();
            lastChunkCoords = ChunkCoords;
        }
        private void LateUpdate()
        {
            if (ChunkCoords != lastChunkCoords)
            {
                // CHUNK LOADER HAS MOVED
                Assert();
                lastChunkCoords = ChunkCoords;
            }
        }

        void Assert()
        {
            if (World.instance == null)
                return;

            foreach (Vector3Int c in Bounds.allPositionsWithin)
            {
                World.instance.AssertChunkLoader(c, this);
            }
        }
    }
}
