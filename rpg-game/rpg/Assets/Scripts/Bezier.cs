using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    public static Vector2 Quadratic(Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        return Vector2.Lerp(Vector2.Lerp(p1, p2, t), Vector2.Lerp(p2, p3, t), t);
    }
}
