using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RPG.Editor.Entities
{
    /// <summary>
    /// Attach this to a prefab instance to save it in the world
    /// </summary>
    public class Prop : Entity
    {
        public override void OnSerialize(JObject json)
        {
            // Serialize prefab name
            base.OnSerialize(json);
            // There's no reason this should be called in play mode
            json["prefabName"] = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject.transform.GetChild(0));
        }
        public override void OnDeserialize(JObject json)
        {
            base.OnDeserialize(json);

            string path = json["prefabName"].ToString();
            if (path.Length < 16)
            {
                GameObject.DestroyImmediate(gameObject);
                return;
            }
            path = path.Remove(0, 17);  // Remove "Assets/Resources/" from string
            path = path.Remove(path.Length - 7, 7);  // Remove the ".prefab" extension
            GameObject prefab = Resources.Load<GameObject>(path);
            if (Application.isEditor)
            {
                PrefabUtility.InstantiatePrefab(prefab, transform).hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                GameObject.Instantiate(prefab, transform);
            }
        }
        public void OnDrawGizmos()
        {
            TileTransform tile = transform.GetComponentInChildren<TileTransform>();
            if (tile != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube((Vector3Int.FloorToInt(transform.position) + new Vector3(0.5f, tile.size.y / 2.0f, 0.5f)), tile.size);
            }
        }
    }
}
