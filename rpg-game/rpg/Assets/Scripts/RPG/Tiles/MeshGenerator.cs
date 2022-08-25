using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Mesh Generator for the world
/// </summary>
public static class MeshGenerator
{
    static MeshGenerator()
    {
        foreach (TileShape shape in Resources.LoadAll<TileShape>("Tiles/Shapes"))
        {
            shape.Init();
        }
    }
    public static void Generate(World world)
    {
        // Foreach tile's neighbors
        // If neighbor is NOT blocking current tile face
        // Create face

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        int indexOffset = 0;

        void AddTriangles(List<int> otherIndices, List<Vector3> otherVertices, Vector3 offset)
        {
            foreach (Vector3 vertex in otherVertices)
            {
                indices.Add(vertices.Count);
                vertices.Add((vertex + offset) * World.tileSize);
            }
        }

        for (int i = 0; i < world.mapVolume; i++)
        {
            Vector3Int tileCoords = world.UnFlattenIndex(i);
            // Get tile
            Tile tile = world.GetTile(tileCoords);

            TileShape shape = tile.Shape;
            if (shape != null)  // If this tile has a mesh to insert
            {
                // Insert mesh
                Vector3 vertexOffset = (Vector3)tileCoords;
                AddTriangles(shape.indices, shape.vertices, vertexOffset);

                for (int d = 0; d < 6; d++)
                {
                    // Get neighbor tile
                    Vector3Int direction = Tile.neighbors[d];
                    Vector3Int neighborCoords = tileCoords + direction;
                    Tile neighborTile = world.GetTile(neighborCoords);

                    if (neighborTile.IsSolid == false)  // If the neighbor is NOT solid
                    {
                        // Insert the triangles on this face
                        AddTriangles(shape.faces[d].indices, shape.faces[d].vertices, vertexOffset);
                    }
                }
                
            }
        }

        MeshFilter filter;
        if (world.gameObject.TryGetComponent<MeshFilter>(out filter) == false)
        {
            filter = world.gameObject.AddComponent<MeshFilter>();
        }

        MeshRenderer renderer;
        if (world.gameObject.TryGetComponent<MeshRenderer>(out renderer) == false)
        {
            renderer = world.gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = Resources.Load<Material>("Wall");
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = indices.Count >= 32684 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        filter.sharedMesh = mesh;
    }
}
