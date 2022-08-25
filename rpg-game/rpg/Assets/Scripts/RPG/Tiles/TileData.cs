using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="Tile Data", menuName = "RPG/Tile Data")]
public class TileData : ScriptableObject
{
    public bool solid;  // Whether the tile blocks movement, sight
    public TileShape shape;  // The mesh of the tile
}
