using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
public class ResourceSelectionWindow : EditorWindow
{
    List<ScriptableObject> resources;
    public System.Type type;
    int selectedIndex = 0;
    public void SetType(System.Type type)
    {
        this.type = type;
    }
    public void Reload()
    {
        resources = new List<ScriptableObject>();

        foreach (ScriptableObject instance in Resources.LoadAll<ScriptableObject>(""))
        {
            if (type == instance.GetType())
                resources.Add(instance);
        }

        base.titleContent = new GUIContent(type.ToString());
    }
    private void OnGUI()
    {
        GUILayout.Label($"{(resources == null ? "No resources" : $"{resources.Count} found")}");

        if (resources != null)
        {
            GUIContent[] content = new GUIContent[resources.Count];
            for (int i = 0; i < content.Length; i++)
            {
                GUIContent gui = null;
                ScriptableObject obj = resources[i];
                if (obj is RPG.TileMaterial material)
                {
                    gui = new GUIContent(material.unityMaterial.mainTexture);
                }
                else if (obj is RPG.TileShape shape)
                {
                    gui = new GUIContent(AssetPreview.GetAssetPreview(shape.mesh));
                }
                content[i] = gui;
            }

            selectedIndex = GUILayout.SelectionGrid(selectedIndex, content, 4, GUILayout.Height(100));
        }
    }
    public T GetSelected<T>() where T : ScriptableObject
    {
        return resources[selectedIndex] as T;
    }
}