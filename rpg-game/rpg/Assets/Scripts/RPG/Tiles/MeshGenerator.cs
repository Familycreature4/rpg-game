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

        public static void Generate(Chunk chunk)
        {
            // Foreach tile's neighbors
            // If neighbor is NOT blocking current tile face
            // Create face

            List<Vertex> vertices = new List<Vertex>();
            Dictionary<UnityEngine.Material, IndexBuffer> materials = new Dictionary<Material, IndexBuffer>();
            int slotCounter = 0;

            void InsertMesh(ref Tile tile, ref Vector3Int localCoords)
            {
                // Ensure material slot exists in mesh
                Material material = tile.material.unityMaterial;
                if (materials.TryGetValue(material, out IndexBuffer indexBuffer) == false)
                {
                    // Create index buffer
                    indexBuffer = new IndexBuffer();
                    indexBuffer.indices = new List<int>();
                    indexBuffer.materialSlot = slotCounter;
                    materials.Add(material, indexBuffer);
                    slotCounter++;
                }

                // Create the solid mask
                int solidMask = 0;
                for (int i = 0; i < 6; i++)
                {
                    bool solid = World.Current.GetTile(localCoords + Vector3Int.RoundToInt(tile.rotation * Tile.neighbors[i]) + chunk.coords * Chunk.size).IsSolid;
                    if (solid)
                        solidMask |= 1 << i;
                }

                // Add triangles
                for (int t = 0; t < tile.shape.vertices.Count / 3; t++)  // For each triangle
                {
                    // Check if adjacent block should be occluding this triangle
                    int faceIndex = tile.shape.triangleFaces[t];
                    if (faceIndex != -1 && ((1 << faceIndex) & solidMask) == (1 << faceIndex))
                        continue;

                    // Determine the UV mapping offsets

                    Vector3 normal = Vector3.Cross(
                        (tile.shape.vertices[t * 3 + 1].position - tile.shape.vertices[t * 3].position).normalized,
                        ((tile.shape.vertices[t * 3 + 2].position - tile.shape.vertices[t * 3].position).normalized)).normalized;

                    float up = Mathf.Abs(Vector3.Dot(Vector3.up, normal));
                    float right = Mathf.Abs(Vector3.Dot(Vector3.right, normal));
                    float forward = Mathf.Abs(Vector3.Dot(Vector3.forward, normal));

                    Vector3 directions = new Vector3(right, up, forward);

                    float uOffset = Mathf.Abs(Vector3.Dot(directions, new Vector3(localCoords.z, localCoords.x, localCoords.x)));
                    float vOffset = Mathf.Abs(Vector3.Dot(directions, new Vector3(localCoords.y, localCoords.z, localCoords.y)));

                    // Insert vertices
                    for (int v = 0; v < 3; v++)
                    {
                        Vertex vertex = tile.shape.vertices[t * 3 + v];
                        indexBuffer.indices.Add(vertices.Count);
                        Vertex newVertex = vertex;

                        newVertex.position = ((tile.rotation * (vertex.position - Vector3.one / 2.0f)) + Vector3.one / 2.0f + localCoords);

                        // Scale UV

                        Vector2 queryUv = vertex.uv + new Vector2(uOffset, vOffset);
                        newVertex.uv = queryUv;

                        vertices.Add(newVertex);
                    }
                    
                }
            }

            for (int i = 0; i < Chunk.sizeCubed; i++)
            {
                Vector3Int localCoords = Chunk.UnFlattenIndex(i);
                Vector3Int worldCoords = localCoords + chunk.coords * Chunk.size;
                // Get tile
                Tile tile = chunk.GetTile(localCoords);

                TileShape shape = tile.shape;
                if (shape != null)  // If this tile has a mesh to insert
                {
                    // Insert mesh
                    InsertMesh(ref tile, ref localCoords);
                }
            }

            // Mesh has been created
            // Apply to gameobject

            Mesh mesh = new Mesh();
            mesh.SetVertexBufferParams(vertices.Count, attributes);
            mesh.SetVertexBufferData<Vertex>(vertices, 0, 0, vertices.Count);

            mesh.subMeshCount = materials.Count;
            foreach (KeyValuePair<Material, IndexBuffer> pair in materials)
            {
                mesh.SetIndices(pair.Value.indices, MeshTopology.Triangles, pair.Value.materialSlot);
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            chunk.component.MeshFilter.sharedMesh = mesh;
            chunk.component.Collider.sharedMesh = mesh;

            // Set materials
            Material[] meshMaterials = new Material[materials.Count];
            foreach (KeyValuePair<Material, IndexBuffer> pair in materials)
            {
                meshMaterials[pair.Value.materialSlot] = pair.Key;
            }

            chunk.component.MeshRenderer.sharedMaterials = meshMaterials;
        }

        public struct Vertex
        {
            public Vector3 position;
            public Vector3 normal;
            public Vector2 uv;
        }
        public class IndexBuffer
        {
            public List<int> indices;
            public int offset = 0;
            public int materialSlot = 0;
        }
    }
}