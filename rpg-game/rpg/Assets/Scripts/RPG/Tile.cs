using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public static Tile Air => new Tile { data = null };
    public Tile(TileData data)
    {
        this.data = data;
    }
    public Tile()
    {

    }
    public bool IsSolid => data == null ? false : data.solid;
    public TileData data;
}
