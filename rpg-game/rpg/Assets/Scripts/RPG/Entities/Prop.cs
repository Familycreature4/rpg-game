using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RPG.Entities
{
    /// <summary>
    /// Attach this to a prefab instance to save it in the world
    /// </summary>
    public class Prop : Entity
    {
        public void SetPrefab(GameObject prefab)
        {
            if (Application.isEditor)
            {
#if UNITY_EDITOR
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = transform.GetChild(i);
                    if (PrefabUtility.IsPartOfPrefabInstance(child))
                    {
                        GameObject.DestroyImmediate(PrefabUtility.GetNearestPrefabInstanceRoot(child).gameObject);
                    }
                }

                GameObject go = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
                go.hideFlags = HideFlags.HideAndDontSave;

                foreach (Transform t in go.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
#endif
            }
            else
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Transform child = transform.GetChild(i);
                    GameObject.Destroy(child.gameObject);
                }

                GameObject.Instantiate(prefab, transform);
            }
        }
        void DeleteChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (Application.isEditor)
                {
                    GameObject.DestroyImmediate(child.gameObject);
                }
                else
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }
        public override void OnSerialize(JObject json)
        {
#if UNITY_EDITOR
            // Serialize prefab name
            base.OnSerialize(json);
            // There's no reason this should be called in play mode
            json["prefabName"] = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject.transform.GetChild(0));
#endif
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

            path = RPG.Editor.Serializer.AssetToResourcePath(path);
            GameObject prefab = Resources.Load<GameObject>(path);
            SetPrefab(prefab);
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
        private void OnDestroy()
        {
            DeleteChildren();
        }
        public override void OnInitialize()
        {
            base.OnInitialize();

            if (Application.isEditor)
            {
#if UNITY_EDITOR
                foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
                {
                    if (PrefabUtility.IsPartOfPrefabInstance(t.gameObject))
                    {
                        t.gameObject.hideFlags = HideFlags.HideInHierarchy;
                    }
                }
#endif
            }
        }
    }
}
