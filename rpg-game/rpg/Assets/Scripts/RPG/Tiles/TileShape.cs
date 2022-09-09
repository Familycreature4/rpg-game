using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Tile Shape", menuName = "RPG/Tile Shape")]
    public class TileShape : ScriptableObject
    {
        static Dictionary<string, TileShape> shapes;
        public static void BuildShapes()
        {
            shapes = new Dictionary<string, TileShape>();

            foreach (TileShape shape in Resources.LoadAll<TileShape>("Tiles/Shapes"))
            {
                shapes.Add(shape.name, shape);
                shape.Init();
            }
        }
        public static TileShape GetShape(string name) => shapes[name];

        public Mesh mesh;
        [HideInInspector]
        public List<MeshGenerator.Vertex> vertices;
        [HideInInspector]
        public List<int> indices;  // Triangles that do not belong to a face (are not coplanar to any face)
        [HideInInspector]
        public Face[] faces;  // Correlates to the Tile neighbors array
        public void Init()
        {
            // Generate faces
            faces = new Face[]
            {
            new Face(Vector3.forward, 1),   // Front
            new Face(Vector3.back, 0),      // Back
            new Face(Vector3.right, 1),     // Right
            new Face(Vector3.left, 0),      // Left
            new Face(Vector3.up, 1),        // Up
            new Face(Vector3.down, 0),      // Down
            };
            vertices = new List<MeshGenerator.Vertex>();
            indices = new List<int>();

            Vector3[] meshVertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            Vector3[] normals = mesh.normals;

            for (int t = 0; t < (mesh.triangles.Length / 3); t++)
            {
                // Determine which face this triangle belongs to, if any

                int t0 = mesh.triangles[t * 3];
                int t1 = mesh.triangles[t * 3 + 1];
                int t2 = mesh.triangles[t * 3 + 2];

                Vector3 v1 = meshVertices[t0];
                Vector3 v2 = meshVertices[t1];
                Vector3 v3 = meshVertices[t2];

                Plane trianglePlane = new Plane(v1, v2, v3);

                // Decide which list of indices to assign this triangle to
                List<int> activeIndices = indices;
                List<MeshGenerator.Vertex> activeVertices = vertices;

                for (int d = 0; d < 6; d++)
                {
                    Face face = faces[d];
                    if (face.IsCoplanar(trianglePlane))  // If the triangle lies on the face
                    {
                        // Assign indices to this face
                        activeIndices = face.indices;
                        activeVertices = face.vertices;
                        //Debug.DrawLine(v1, v2, Color.white, 10.0f);
                        //Debug.DrawLine(v1, v3, Color.white, 10.0f);
                        //Debug.DrawLine(v2, v3, Color.white, 10.0f);
                        //Debug.DrawRay((v1 + v2 + v3) / 3.0f, Tile.neighbors[d], Color.yellow, 10.0f);
                        break;
                    }
                }

                activeIndices.Add(t0);
                activeIndices.Add(t1);
                activeIndices.Add(t2);

                activeVertices.Add(new MeshGenerator.Vertex { position = v1, normal = normals[t0], uv = uvs[t0] });
                activeVertices.Add(new MeshGenerator.Vertex { position = v2, normal = normals[t1], uv = uvs[t1] });
                activeVertices.Add(new MeshGenerator.Vertex { position = v3, normal = normals[t2], uv = uvs[t2] });
            }
        }
        public class Face
        {
            public Face(Vector3 normal, float distance)
            {
                indices = new List<int>();
                vertices = new List<MeshGenerator.Vertex>();
                plane = new Plane(normal, distance);
            }

            public List<int> indices;  // Triangles which are coplanar to this face
            public List<MeshGenerator.Vertex> vertices;
            Plane plane;

            public bool IsCoplanar(Plane plane)
            {
                return Mathf.Abs(this.plane.distance) == Mathf.Abs(plane.distance) && Mathf.Abs(Vector3.Dot(plane.normal, this.plane.normal)) == 1;
            }
        }
    }
}