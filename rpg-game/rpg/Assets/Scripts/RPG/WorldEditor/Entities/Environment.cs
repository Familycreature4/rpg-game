using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Editor.Entities
{
    public class Environment : Entity
    {
        public Material skyboxMaterial;

        public override void OnSerialize(JObject json)
        {
            base.OnSerialize(json);

            json["material"] = UnityEditor.AssetDatabase.GetAssetPath(skyboxMaterial);
        }

        public override void OnDeserialize(JObject json)
        {
            base.OnDeserialize(json);

            skyboxMaterial = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Material>(json["material"].ToString());

            ApplyMaterial();
        }
        public void ApplyMaterial()
        {
            UnityEngine.RenderSettings.skybox = skyboxMaterial;
        }
    }
}
