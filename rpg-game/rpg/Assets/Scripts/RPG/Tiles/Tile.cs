using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [System.Serializable]
    public struct Tile
    {
        public readonly static Tile Air = new Tile {  };
        public static Vector3Int[] neighbors =
        {
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };
        public Tile(string mat)
        {
            shape = TileShape.GetShape("Cube");
            material = TileMaterial.GetMaterial(mat);
            rotation = Quaternion.identity;
        }
        public bool IsSolid => material != null && shape != null;
        public TileShape shape;
        public TileMaterial material;
        public Quaternion rotation;  // The orientation of this tile
    }
}