using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Editor.Entities;
[CustomEditor(typeof(RPG.Editor.Entities.Environment))]
public class EnvironmentInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        Material mat = EditorGUILayout.ObjectField("Skybox Material", (target as Environment).skyboxMaterial, typeof(Material), false) as Material;
        if (EditorGUI.EndChangeCheck())
        {
            (target as Environment).skyboxMaterial = mat;
            (target as Environment).ApplyMaterial();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
