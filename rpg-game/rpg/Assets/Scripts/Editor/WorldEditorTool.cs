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
                    return window.GetSelected<TileMaterial>();
                }
            }

            return TileMaterial.GetMaterial("Marble Tile");
        }
    }
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
                if (Event.current.keyCode == KeyCode.LeftShift)
                {
                    shift = true;
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
                        World.Current.SetTile(Vector3Int.FloorToInt(cursorPos + cursorNormal * 0.01f), new Tile(SelectedMaterial));
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
                Handles.color = Color.red;
                Handles.ArrowHandleCap(0, cursorPos + cursorNormal * 1.2f, Quaternion.LookRotation(-cursorNormal), 1.0f, EventType.Repaint);
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
