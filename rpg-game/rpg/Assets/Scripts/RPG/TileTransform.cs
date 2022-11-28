using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// Places the GameObject into the Tile World
    /// </summary>
    public class TileTransform : MonoBehaviour
    {
        public static List<TileTransform> transforms = new List<TileTransform>();
        public RPG.Bounds Bounds => new RPG.Bounds(this);
        public Vector3Int coordinates;
        public Vector3Int size = Vector3Int.one;
        public void Awake()
        {
            transforms.Add(this);
            coordinates = Vector3Int.FloorToInt(transform.position);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube((Vector3Int.FloorToInt(transform.position) + new Vector3(0.5f, size.y / 2.0f, 0.5f)), size);
            //Gizmos.DrawWireCube(Bounds.mins + (Vector3)Bounds.size / 2.0f, (Vector3)size);
        }
    }
}