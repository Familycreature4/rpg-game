using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    [System.Serializable]
    public struct Tile
    {
        public readonly static Tile Air = new Tile {  };
        public readonly static Vector3Int[] neighbors =
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
        };
        public static int GetNeighborIndex(Vector3Int direction)
        {
            for (int i = 0; i < 6; i++)
            {
                if (direction == neighbors[i])
                {
                    return i;
                }
            }

            return -1;
        }
        public Tile(string mat)
        {
            shape = TileShape.GetShape("Cube");
            material = TileMaterial.GetMaterial(mat);
            rotation = Quaternion.identity;
        }
        public Tile(TileMaterial mat)
        {
            shape = TileShape.GetShape("Cube");
            material = mat;
            rotation = Quaternion.identity;
        }
        public bool IsSolid => material != null && shape != null;
        public bool IsClimbable => shape != null && shape.climbable;
        public TileShape shape;
        public TileMaterial material;
        public Quaternion rotation;  // The orientation of this tile
        public override string ToString()
        {
            string tile = "Tile\n";
            if (material != null)
                tile += $"Material: {material.name}\n";
            if (shape != null)
                tile += $"Shape: {shape.name}\n";

            tile += $"Rotation: {rotation.eulerAngles}";

            return tile;
        }
    }
}