using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [CreateAssetMenu(fileName = "Tile Material", menuName = "RPG/Tile Material")]
    public class TileMaterial : ScriptableObject
    {
        public static Dictionary<string, TileMaterial> materials;
        public static string[] materialKeys;
        public static int Count => materials.Count;
        public static void BuildMaterials()
        {
            materials = new Dictionary<string, TileMaterial>();

            TileMaterial[] tileMaterials = Resources.LoadAll<TileMaterial>("Tiles/Materials");
            materialKeys = new string[tileMaterials.Length];
            for (int i = 0; i < tileMaterials.Length; i++)
            {
                TileMaterial mat = tileMaterials[i];
                materialKeys[i] = mat.name;
                if (mat.unityMaterial.mainTexture != null)
                {
                    Vector2 scaling = new Vector2(mat.pixelDensity.x / mat.unityMaterial.mainTexture.width, mat.pixelDensity.y / mat.unityMaterial.mainTexture.height);
                    mat.unityMaterial.mainTextureScale = scaling;
                }
                materials.Add(mat.name, mat);
            }
        }
        public static TileMaterial GetMaterial(string name) => materials[name];
        public UnityEngine.Material unityMaterial;
        public Vector2 pixelDensity = new Vector2(128, 128);
    }
}
