using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Tile Shape", menuName = "RPG/Tile Shape")] [System.Serializable] 
    public class TileShape : ScriptableObject
    {
        public static int Count => shapes.Count;
        public static Dictionary<string, TileShape> shapes;
        public static void BuildShapes()
        {
            shapes = new Dictionary<string, TileShape>();

            foreach (TileShape shape in Resources.LoadAll<TileShape>("Tiles/Shapes"))
            {
                shapes.Add(shape.name, shape);
                shape.Init();
            }
        }
        public static TileShape GetShape(string name)
        {
            return shapes[name];
        }
        private void OnEnable()
        {
            if (shapes == null)
                BuildShapes();
        }
        readonly static Plane[] planes = new Plane[6]
        {
            new Plane { normal = Vector3.forward, distance = 1},
            new Plane { normal = Vector3.back, distance = 0},
            new Plane { normal = Vector3.right, distance = 1},
            new Plane { normal = Vector3.left, distance = 0},
            new Plane { normal = Vector3.up, distance = 1},
            new Plane { normal = Vector3.down, distance = 0}
        };

        public Mesh mesh;
        [System.NonSerialized]
        public List<MeshGenerator.Vertex> vertices;
        [System.NonSerialized]
        public List<int> indices;
        [System.NonSerialized]
        public int[] triangleFaces;
        public void Init()
        {
            vertices = new List<MeshGenerator.Vertex>();
            indices = new List<int>();

            Vector3[] meshVertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            Vector3[] normals = mesh.normals;

            int triangleCount = mesh.triangles.Length / 3;
            triangleFaces = new int[triangleCount];

            for (int i = 0; i < triangleFaces.Length; i++)
                triangleFaces[i] = -1;

            for (int t = 0; t < triangleCount; t++)
            {
                // Determine which face this triangle belongs to, if any

                int t0 = mesh.triangles[t * 3];
                int t1 = mesh.triangles[t * 3 + 1];
                int t2 = mesh.triangles[t * 3 + 2];

                Vector3 v1 = meshVertices[t0];
                Vector3 v2 = meshVertices[t1];
                Vector3 v3 = meshVertices[t2];

                bool IsCoplanar(ref Plane plane)
                {
                    Plane myPlane = new Plane(v1, v2, v3);
                    return Mathf.Abs(myPlane.distance) == Mathf.Abs(plane.distance) && Mathf.Abs(Vector3.Dot(plane.normal, myPlane.normal)) == 1;
                }

                for (int i = 0; i < 6; i++)
                {
                    if (IsCoplanar(ref planes[i]))
                    {
                        triangleFaces[t] = i;
                        break;
                    }
                }

                indices.Add(t0);
                indices.Add(t1);
                indices.Add(t2);

                vertices.Add(new MeshGenerator.Vertex { position = v1, normal = normals[t0], uv = uvs[t0] });
                vertices.Add(new MeshGenerator.Vertex { position = v2, normal = normals[t1], uv = uvs[t1] });
                vertices.Add(new MeshGenerator.Vertex { position = v3, normal = normals[t2], uv = uvs[t2] });
            }
        }
    }
}