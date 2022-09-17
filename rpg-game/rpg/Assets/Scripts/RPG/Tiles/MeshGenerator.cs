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

            void AddTriangles(List<int> otherIndices, List<Vertex> otherVertices, Vector3 offset, TileMaterial material)
            {
                Atlas.Bounds bounds = Atlas.GetBounds((Texture2D)material.texture);

                foreach (Vertex vertex in otherVertices)
                {
                    indices.Add(vertices.Count);
                    Vertex newVertex = vertex;

                    newVertex.position = (vertex.position + offset);
                    // Scale/offset uv
                    // Remap the mesh uvs to the uv range in the material
                    newVertex.uv = new Vector2(
                        Utilities.MapRange(vertex.uv.x * material.uvScale.x, 0, 1.0f, bounds.UvMin.x, bounds.UvMax.x),
                        Utilities.MapRange(vertex.uv.y * material.uvScale.y, 0, 1.0f, bounds.UvMin.y, bounds.UvMax.y)
                    );

                    vertices.Add(newVertex);
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
                    Vector3 vertexOffset = (Vector3)localCoords;
                    AddTriangles(shape.indices, shape.vertices, vertexOffset, tile.material);

                    for (int d = 0; d < 6; d++)
                    {
                        // Get neighbor tile
                        Vector3Int direction = Tile.neighbors[d];
                        Vector3Int neighborCoords = worldCoords + direction;
                        if (World.instance.IsSolid(neighborCoords) == false || IsNodraw(neighborCoords))  // If the neighbor is NOT solid
                        {
                            // Insert the triangles on this face
                            AddTriangles(shape.faces[d].indices, shape.faces[d].vertices, vertexOffset, tile.material);
                        }
                    }

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