using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [CreateAssetMenu(fileName = "Tile Material", menuName = "RPG/Tile Material")]
    public class TileMaterial : ScriptableObject
    {
        public static Dictionary<string, TileMaterial> materials;
        public static Texture2D atlas;
        public static void BuildMaterials()
        {
            materials = new Dictionary<string, TileMaterial>();

            foreach (TileMaterial mat in Resources.LoadAll<TileMaterial>("Tiles/Materials"))
            {
                materials.Add(mat.name, mat);
            }

            Atlas.GenerateTextureAtlas();
        }
        public static TileMaterial GetMaterial(string name) => materials[name];
        public Texture texture;
        public Vector2 uvScale = Vector3.one;
    }
}
