using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace RPG
{
    /// <summary>
    /// Mesh Generator for the world
    /// </summary>
    public static class MeshGenerator
    {
        static VertexAttributeDescriptor[] attributes = new VertexAttributeDescriptor[]
        {
            new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
            new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 0),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 0),
        };

        public static void Generate(Chunk chunk, List<BoundsInt> nodraws = null)
        {
            // Foreach tile's neighbors
            // If neighbor is NOT blocking current tile face
            // Create face

            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();

            void InsertMesh(ref Tile tile, ref Vector3Int localCoords)
            {
                Atlas.Bounds bounds = Atlas.GetBounds((Texture2D)tile.material.texture);

                int solidMask = 0;
                for (int i = 0; i < 6; i++)
                {
                    bool solid = World.Current.GetTile(localCoords + Vector3Int.RoundToInt(tile.rotation * Tile.neighbors[i]) + chunk.coords * Chunk.size).IsSolid;
                    if (solid)
                        solidMask |= 1 << i;
                }

                for (int t = 0; t < tile.shape.vertices.Count / 3; t++)
                {
                    int faceIndex = tile.shape.triangleFaces[t];
                    // Check if adjacent block should be occluding this triangle
                    if (faceIndex != -1 && ((1 << faceIndex) & solidMask) == (1 << faceIndex))
                        continue;

                    for (int v = 0; v < 3; v++)
                    {
                        Vertex vertex = tile.shape.vertices[t * 3 + v];
                        indices.Add(vertices.Count);
                        Vertex newVertex = vertex;

                        newVertex.position = ((tile.rotation * (vertex.position - Vector3.one / 2.0f)) + Vector3.one / 2.0f + localCoords);

                        // Scale/offset uv
                        // Remap the mesh uvs to the uv range in the material

                        Vector2 queryUv = vertex.uv;

                        Vector2Int uvOffsets = new Vector2Int(
                            (int)Utilities.Mod(localCoords.x + localCoords.y, 1.0f / (tile.material.uvScale.x)),
                            (int)Utilities.Mod(localCoords.z + localCoords.y, 1.0f / (tile.material.uvScale.y))
                        );

                        queryUv += uvOffsets;

                        queryUv.x *= tile.material.uvScale.x;
                        queryUv.y *= tile.material.uvScale.y;

                        newVertex.uv = new Vector2(
                            Utilities.MapRange(queryUv.x, 0, 1.0f, bounds.UvMin.x, bounds.UvMax.x),
                            Utilities.MapRange(queryUv.y, 0, 1.0f, bounds.UvMin.y, bounds.UvMax.y)
                        );

                        vertices.Add(newVertex);
                    }
                    
                }
            }

            bool IsNodraw(Vector3Int c)
            {
                // Check if it does not lie inside a nodraw bounds
                if (nodraws != null)
                {
                    foreach (BoundsInt nodraw in nodraws)
                    {
                        if (nodraw.Contains(c))
                            return true;
                    }
                }

                return false;
            }

            for (int i = 0; i < Chunk.sizeCubed; i++)
            {
                Vector3Int localCoords = Chunk.UnFlattenIndex(i);
                Vector3Int worldCoords = localCoords + chunk.coords * Chunk.size;
                // Get tile
                Tile tile = chunk.GetTile(localCoords);

                TileShape shape = tile.shape;
                if (shape != null && IsNodraw(worldCoords) == false)  // If this tile has a mesh to insert
                {
                    // Insert mesh
                    InsertMesh(ref tile, ref localCoords);
                }
            }

            // Mesh has been created
            // Apply to gameobject

            Mesh mesh = new Mesh();
            //IndexFormat format = indices.Count >= 65536 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            mesh.SetVertexBufferParams(vertices.Count, attributes);
            mesh.SetVertexBufferData<Vertex>(vertices, 0, 0, vertices.Count);
            mesh.SetIndexBufferParams(indices.Count, UnityEngine.Rendering.IndexFormat.UInt32);
            mesh.SetIndexBufferData(indices, 0, 0, indices.Count, MeshUpdateFlags.Default);
            mesh.SetSubMesh(0, new SubMeshDescriptor { baseVertex = 0, firstVertex = 0, indexCount = indices.Count, vertexCount = vertices.Count });

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            chunk.component.MeshFilter.sharedMesh = mesh;
            chunk.component.Collider.sharedMesh = mesh;
        }

        public struct Vertex
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 uv;
        }
    }
}