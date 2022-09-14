using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RPG
{
    public class WorldEditor : EditorWindow
    {
        public static WorldEditor instance;
        [MenuItem("RPG/World Editor")]
        public static void ShowWindow()
        {
            instance = (WorldEditor)GetWindow(typeof(WorldEditor));
        }

        private void OnGUI()
        {
            GUILayout.Label("World Editor", EditorStyles.label);

            if (GUILayout.Button("Create From Image"))
            {
                Debug.Log("WORLD");
            }
        }
    }
}