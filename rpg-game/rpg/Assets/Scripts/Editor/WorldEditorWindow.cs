using UnityEngine;
using UnityEditor;
using RPG;

public class WorldEditorWindow : EditorWindow
{
    [MenuItem("Window/World Editor")]
    public static void ShowWindow()
    {
        WorldEditorWindow window = EditorWindow.GetWindow(typeof(WorldEditorWindow)) as WorldEditorWindow;
        window.ReloadWorld();
    }

    World world;
    void ReloadWorld()
    {
        world = World.Current;

        if (world != null)
        {
            ReloadResources();
            world.DirtyChunks();
        }
        // TILE SHAPE
    }
    void ReloadResources()
    {
        TileMaterial.BuildMaterials();
        TileShape.BuildShapes();
    }
    private void OnGUI()
    {
        if (world == null)
        {
            ReloadWorld();
        }

        GUILayout.Label($"World Status: {(world == null ? "NO WORLD FOUND\nEnsure a gameobject with the RPG.World component is present" : $"World found - {world.gameObject.name} ({(world.Chunks == null ? 0 : world.Chunks.Count)} Chunks)")}");
        GUILayout.Label($"Tile Materials: {(TileMaterial.materials == null ? "NO MATERIALS BUILT, RELOAD RESOURCES" : $"{TileMaterial.Count} Materials found")}");
        GUILayout.Label($"Tile Shapes: {(TileShape.shapes == null ? "NO SHAPES BUILT, RELOAD RESOURCES" : $"{TileShape.Count} Shapes found")}");

        if (world != null)
        {
            if (GUILayout.Button("Reload Resources"))
            {
                ReloadResources();
            }
            if (GUILayout.Button("Load World"))
            {
                ReloadResources();
                RPG.Editor.Serializer.LoadWorldExplorer(world);
            }
            if (GUILayout.Button("Save World"))
            {
                ReloadResources();
                RPG.Editor.Serializer.SaveWorldExplorer(world);
            }
            if (GUILayout.Button("Material Explorer"))
            {
                ResourceSelectionWindow window = EditorWindow.GetWindow<ResourceSelectionWindow>();
                window.SetType(typeof(TileMaterial));
                window.Reload();
            }
        }
    }
    void WorldUpdate()
    {

    }
}