using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPG.Editor.Entities;
[CustomEditor(typeof(RPG.Editor.Entities.Prop))]
public class PropInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Prop targetProp = target as RPG.Editor.Entities.Prop;
        GameObject prefab = null;
        if (targetProp.gameObject.transform.childCount > 0)
        {
            prefab = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(targetProp.gameObject.transform.GetChild(0).gameObject);
        }

        EditorGUI.BeginChangeCheck();
        Object prefabObject = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())  // New prefab set
        {
            // Replace existing prefab with new one
            for (int i = targetProp.gameObject.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(targetProp.gameObject.transform.GetChild(i).gameObject);
            }

            targetProp.SetPrefab(prefabObject as GameObject);
        }
        serializedObject.ApplyModifiedProperties();
    }
}
