using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
namespace RPG.Editor
{
    [System.Serializable]
    public class Selection
    {
        public Selection()
        {

        }
        public BoundsInt BoundsInt => new BoundsInt(
            Mathf.FloorToInt(bounds.min.x),
            Mathf.FloorToInt(bounds.min.y),
            Mathf.FloorToInt(bounds.min.z),
            Mathf.CeilToInt(bounds.size.x),
            Mathf.CeilToInt(bounds.size.y),
            Mathf.CeilToInt(bounds.size.z));
        public UnityEngine.Bounds bounds;
        public System.Func<bool> onSelect;

        public IEnumerable<Vector3Int> GetCoords()
        {
            foreach (Vector3Int c in BoundsInt.allPositionsWithin)
                yield return c;
        }
        public void OnGUI(EditorWindow window)
        {
            // Draw the real volume
            Handles.color = Color.red;
            BoundsInt intBounds = BoundsInt;
            Handles.DrawWireCube(intBounds.center, intBounds.size);

            EditorGUI.BeginChangeCheck();
            Vector3 pos = Handles.PositionHandle(bounds.center, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.center = pos;
            }

            #region Bound Handles

            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.zAxisColor;
            Vector3 forwardPos = Handles.Slider(bounds.center + new Vector3(0, 0, bounds.size.z / 2.0f), Vector3.forward);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.max = new Vector3(bounds.max.x, bounds.max.y, forwardPos.z);
            }

            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.zAxisColor;
            Vector3 backwardPos = Handles.Slider(bounds.center + new Vector3(0, 0, -bounds.size.z / 2.0f), -Vector3.forward);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.min = new Vector3(bounds.min.x, bounds.min.y, backwardPos.z);
            }

            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.xAxisColor;
            Vector3 rightPos = Handles.Slider(bounds.center + new Vector3(bounds.size.x / 2.0f, 0, 0), Vector3.right);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.max = new Vector3(rightPos.x, bounds.max.y, bounds.max.z);
            }

            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.xAxisColor;
            Vector3 leftPos = Handles.Slider(bounds.center + new Vector3(-bounds.size.x / 2.0f, 0, 0), -Vector3.right);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.min = new Vector3(leftPos.x, bounds.min.y, bounds.min.z);
            }

            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.yAxisColor;
            Vector3 upPos = Handles.Slider(bounds.center + new Vector3(0, bounds.size.y / 2.0f, 0), Vector3.up);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.max = new Vector3(bounds.max.x, upPos.y, bounds.max.z);
            }

            EditorGUI.BeginChangeCheck();
            Handles.color = Handles.yAxisColor;
            Vector3 downPos = Handles.Slider(bounds.center + new Vector3(0, -bounds.size.y / 2.0f, 0), -Vector3.up);
            if (EditorGUI.EndChangeCheck())
            {
                bounds.min = new Vector3(bounds.min.x, downPos.y, bounds.min.z);
            }

            #endregion

            if (Handles.Button(bounds.max, Quaternion.identity, 1.0f, 1.0f, Handles.RectangleHandleCap))
            {
                onSelect?.Invoke();
            }
        }
    }
}