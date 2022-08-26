using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WorldEditor : ScriptableWizard
{
    [MenuItem("RPG/World Editor")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<WorldEditor>("Create World");
    }

    private void OnWizardCreate()
    {
        if (World.instance == null)
        {
            World.instance = GameObject.FindObjectOfType<World>();
            if (World.instance == null)
                World.instance = new GameObject("World").AddComponent<World>();
        }

        WorldGenerator.GenerateMap(World.instance);
        MeshGenerator.Generate(World.instance);
    }
}
