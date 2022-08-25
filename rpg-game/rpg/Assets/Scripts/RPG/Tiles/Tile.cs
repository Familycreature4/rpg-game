using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public readonly static Tile Air = new Tile { data = null };
    public static Vector3Int[] neighbors =
    {
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
    };
    public Tile(TileData data)
    {
        this.data = data;
    }
    public Tile()
    {

    }
    public bool IsSolid => data == null ? false : data.solid;
    public TileShape Shape => data == null ? null : data.shape;
    public TileData data;
    public Quaternion rotation;  // The orientation of this tile
}
