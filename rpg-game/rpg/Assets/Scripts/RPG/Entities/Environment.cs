using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Entities
{
    public class Environment : Entity
    {
        public Material skyboxMaterial;

        public override void OnSerialize(JObject json)
        {
#if UNITY_EDITOR
            base.OnSerialize(json);
            json["material"] = Editor.Serializer.AssetToResourcePath(UnityEditor.AssetDatabase.GetAssetPath(skyboxMaterial));
#endif
        }
        public override void OnDeserialize(JObject json)
        {
            base.OnDeserialize(json);

            string materialPath = json["material"].ToString();
            materialPath = Editor.Serializer.AssetToResourcePath(materialPath);
            skyboxMaterial = Resources.Load<Material>(materialPath);

            ApplyMaterial();
        }
        public void ApplyMaterial()
        {
            UnityEngine.RenderSettings.skybox = skyboxMaterial;
        }
    }
}
