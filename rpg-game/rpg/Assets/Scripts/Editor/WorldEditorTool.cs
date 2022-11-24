using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using RPG;

[EditorTool("World Editor", typeof(RPG.World))]
public class WorldEditorTool : EditorTool
{
    [UnityEditor.ShortcutManagement.Shortcut("World Editor Tool", typeof(WorldEditorWindow))]
    static void WorldEditorToolShortcut()
    {
        if (Selection.GetFiltered<World>(SelectionMode.TopLevel).Length > 0)
        {
            ToolManager.SetActiveTool<WorldEditorTool>();
        }
    }
    public TileMaterial SelectedMaterial
    {
        get
        {
            if (EditorWindow.HasOpenInstances<ResourceSelectionWindow>())
            {
                ResourceSelectionWindow window = EditorWindow.GetWindow<ResourceSelectionWindow>();
                if (window.type == typeof(TileMaterial))
                {
                    currentMaterial = window.GetSelected<TileMaterial>();
                }
            }

            return currentMaterial;
        }
    }
    public TileShape SelectedShape
    {
        get
        {
            if (EditorWindow.HasOpenInstances<ResourceSelectionWindow>())
            {
                ResourceSelectionWindow window = EditorWindow.GetWindow<ResourceSelectionWindow>();
                if (window.type == typeof(TileShape))
                {
                    currentShape = window.GetSelected<TileShape>();
                }
            }

            return currentShape;
        }
    }
    TileMaterial currentMaterial = TileMaterial.GetMaterial("Marble Tile");
    TileShape currentShape = TileShape.GetShape("Cube");
    Vector3 tileAngles = Vector3.zero;
    bool shift = false;
    bool didEdit = false;
    int controlId;
    private void OnEnable()
    {
        //EditorWindow.GetWindow(typeof(WorldEditorWindow));
    }
    private void OnDisable()
    {

    }
    public override void OnToolGUI(EditorWindow window)
    {
        if (window is SceneView == false)
            return;

        Ray guiRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        Vector3 cursorPos;
        Vector3 cursorNormal = Vector3.up;

        if (Physics.Raycast(guiRay, out RaycastHit hit))
        {
            cursorPos = hit.point;
            cursorNormal = hit.normal;
        }
        else
        {
            cursorPos = guiRay.origin + guiRay.direction * 10;
        }

        switch (Event.current.type)
        {
            case EventType.KeyDown:
                switch (Event.current.keyCode)
                {
                    case KeyCode.LeftShift:
                        shift = true;
                        break;
                    case KeyCode.Q:
                        tileAngles.y += 90.0f;
                        Event.current.Use();
                        break;
                    case KeyCode.E:
                        tileAngles.y -= 90.0f;
                        Event.current.Use();
                        break;
                }
                break;
            case EventType.KeyUp:
                if (Event.current.keyCode == KeyCode.LeftShift)
                {
                    shift = false;
                }
                break;
            case EventType.MouseMove:
                window.Repaint();
                break;
            case EventType.MouseDown:
                if (Event.current.button == 0)
                {
                    if (shift == false)
                    {
                        World.Current.SetTile(Vector3Int.FloorToInt(cursorPos + cursorNormal * 0.01f), new Tile {
                            material = SelectedMaterial,
                            shape = SelectedShape,
                            rotation = Quaternion.Euler(tileAngles)
                        });
                    }
                    else
                    {
                        World.Current.SetTile(Vector3Int.FloorToInt(cursorPos - cursorNormal * 0.01f), Tile.Air);
                    }
                    didEdit = true;
                    Event.current.Use();
                    window.Focus();
                }
                break;
            case EventType.Layout:
                controlId = GUIUtility.GetControlID(FocusType.Passive);
                HandleUtility.AddDefaultControl(controlId);
                break;
            case EventType.Repaint:
                Vector3 endPos = cursorPos + cursorNormal * 0.75f;

                // Draw origin
                float distance = 1.0f;
                float offset = 1.2f;
                Quaternion rotation = Quaternion.Euler(tileAngles);
                Handles.color = Color.red;
                Handles.ArrowHandleCap(0, cursorPos, rotation * Quaternion.LookRotation(Vector3.right), distance, EventType.Repaint);
                Handles.color = Color.blue;
                Handles.ArrowHandleCap(0, cursorPos, rotation * Quaternion.LookRotation(Vector3.forward), distance, EventType.Repaint);
                Handles.color = Color.green;
                Handles.ArrowHandleCap(0, cursorPos, rotation * Quaternion.LookRotation(Vector3.up), distance, EventType.Repaint);

                GUIStyle style = new GUIStyle();
                GUIContent content = new GUIContent(shift ? "DELETE MODE" : "PLACE MODE");
                style.fontSize = 28;
                Handles.Label(endPos, content, style);
                
                if (didEdit)
                {
                    window.Repaint();
                    didEdit = false;
                }
                break;
        }
    }
}
