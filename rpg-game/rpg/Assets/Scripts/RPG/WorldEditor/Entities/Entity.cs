using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPG.Editor;
using UnityEditor;

namespace RPG.Editor.Entities
{
    public class Entity : MonoBehaviour
    {
        public static void ClearEntities()
        {
            Entity[] entities = GameObject.FindObjectsOfType<Entity>();
            for (int i = entities.Length - 1; i >= 0; i--)
            {
                if (Application.isEditor)
                {
                    GameObject.DestroyImmediate(entities[i].gameObject);
                }
                else
                {
                    GameObject.Destroy(entities[i].gameObject);
                }
                
            }
        }
        private void OnDrawGizmos()
        {
            Vector3Int coords = Vector3Int.FloorToInt(transform.position);
            Gizmos.DrawWireCube(coords + Vector3.one / 2.0f, Vector3.one);
        }
        public virtual void OnSerialize(JObject json)
        {
            json["type"] = GetType().ToString();
            json["name"] = gameObject.name;
            // Write transform
            json.Add("position", Serializer.EncodeVector3(transform.position));
            Vector3 angles = transform.rotation.eulerAngles;
            json.Add("angles", Serializer.EncodeVector3(angles));
        }
        public virtual void OnDeserialize(JObject json)
        {
            // Load transform
            Vector3 position = Serializer.DecodeVector3(json["position"].ToString());
            Quaternion rotation = Quaternion.Euler(Serializer.DecodeVector3(json["angles"].ToString()));

            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
