using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Places the GameObject into the Tile World
/// </summary>
public class TileTransform : MonoBehaviour
{
    public static List<TileTransform> transforms = new List<TileTransform>();
    public Vector3Int coordinates;
    public Vector3Int size = Vector3Int.one;
    public BoundsInt Bounds => new BoundsInt(coordinates.x, coordinates.y, coordinates.z, size.x, size.y, size.z);
    public void Awake()
    {
        transforms.Add(this);
        coordinates = Vector3Int.FloorToInt(transform.position);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(coordinates + new Vector3(0.5f, size.y / 2.0f, 0.5f), (Vector3)size * World.tileSize);
    }
}
