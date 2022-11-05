using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [CreateAssetMenu(fileName = "Tile Material", menuName = "RPG/Tile Material")]
    public class TileMaterial : ScriptableObject
    {
        public static Dictionary<string, TileMaterial> materials;
        public static int Count => materials.Count;
        public static Atlas atlas;
        public static void BuildMaterials()
        {
            materials = new Dictionary<string, TileMaterial>();

            foreach (TileMaterial mat in Resources.LoadAll<TileMaterial>("Tiles/Materials"))
            {
                materials.Add(mat.name, mat);
            }

            atlas = new Atlas();
            //atlas.load = Atlas.Type.Diffuse;
            atlas.GenerateTextureAtlas();
        }
        public static TileMaterial GetMaterial(string name) => materials[name];
        public Texture2D diffuse;
        public Texture2D bump;
        public Texture2D roughness;
        public Texture2D specular;
        public Texture2D normal;
        public float roughnessFallback = 1.0f;
        public float specularFallback = 0.0f;
        public bool deriveSpecularFromRoughness = true;
        public Vector2 scale = Vector2.one;
    }
}
