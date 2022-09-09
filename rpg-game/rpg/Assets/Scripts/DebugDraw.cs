using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugDraw
{
    public static void DrawCube(Vector3 center, Vector3 size, Color color, float time = 0)
    {
        Vector3 c1 = center + new Vector3(size.x, size.y, size.z) / 2;
        Vector3 c2 = center + new Vector3(-size.x, size.y, size.z) / 2;
        Vector3 c3 = center + new Vector3(-size.x, size.y, -size.z) / 2;
        Vector3 c4 = center + new Vector3(-size.x, size.y, size.z) / 2;

        Vector3 c5 = center + -size / 2;
        Vector3 c6 = center + new Vector3(-size.x, -size.y, size.z) / 2;
        Vector3 c7 = center + new Vector3(-size.x, -size.y, -size.z) / 2;
        Vector3 c8 = center + new Vector3(-size.x, -size.y, size.z) / 2;

        Debug.DrawLine(c1, c2, color, time);
        Debug.DrawLine(c2, c3, color, time);
        Debug.DrawLine(c3, c4, color, time);
        Debug.DrawLine(c1, c4, color, time);

        Debug.DrawLine(c5, c6, color, time);
        Debug.DrawLine(c6, c7, color, time);
        Debug.DrawLine(c7, c8, color, time);
        Debug.DrawLine(c5, c8, color, time);

        Debug.DrawLine(c1, c5, color, time);
        Debug.DrawLine(c2, c6, color, time);
        Debug.DrawLine(c3, c7, color, time);
        Debug.DrawLine(c4, c8, color, time);
    }
}
