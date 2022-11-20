using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class ChunkComponent : MonoBehaviour
    {
        public MeshRenderer MeshRenderer
        {
            get
            {
                if (gameObject.TryGetComponent<MeshRenderer>(out MeshRenderer m))
                {
                    return m;
                }
                else
                {
                    m = gameObject.AddComponent<MeshRenderer>();
                    m.material = Resources.Load<Material>("Materials/Atlas");
                    return m;
                }
            }
        }
        public MeshCollider Collider
        {
            get
            {
                if (gameObject.TryGetComponent<MeshCollider>(out MeshCollider m))
                {
                    return m;
                }
                else
                {
                    m = gameObject.AddComponent<MeshCollider>();
                    return m;
                }
            }
        }
        public MeshFilter MeshFilter
        {
            get
            {
                if (gameObject.TryGetComponent(out MeshFilter m))
                {
                    return m;
                }
                else
                {
                    m = gameObject.AddComponent<MeshFilter>();
                    return m;
                }
            }
        }
        public Chunk chunk;
    }
}
