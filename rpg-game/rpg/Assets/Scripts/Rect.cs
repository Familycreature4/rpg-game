using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectPacker
{
    //https://www.codeproject.com/Articles/210979/Fast-optimizing-rectangle-packing-algorithm-for-bu#basic
    public static UnityEngine.Rect[] Pack(Vector2Int[] sizes)
    {
        return null;

        Rect[] rects = new Rect[sizes.Length];
        for (int i = 0; i < rects.Length; i++)
        {
            rects[i] = new Rect{ size = sizes[i], index = i };
        }

        // Sort from largest to smallest dimensions
        System.Array.Sort(rects, delegate (Rect a, Rect b) {
            return Mathf.Max(b.size.x, b.size.y).CompareTo(Mathf.Max(a.size.x, a.size.y));
        });

        int processedRectangles = 0;
        Vector2Int bounds = Vector2Int.zero;

        bool Overlap(int index, Vector2Int position, Vector2Int size)
        {
            Vector2Int max = position + size;
            for (int i = 0; i < processedRectangles; i++)
            {
                if (i == index)
                    continue;

                Rect rect = rects[i];

                if (position.x >= rect.Max.x)
                    continue;
                if (position.y >= rect.Max.y)
                    continue;
                if (max.x < rect.position.x)
                    continue;
                if (max.y < rect.position.y)
                    continue;

                return false;
            }

            return true;
        }
        bool TestOverlap(Vector2Int posA, Vector2Int sizeA, Vector2Int posB, Vector2Int sizeB)
        {
            Vector2Int maxA = posA + sizeA;
            Vector2Int maxB = posB + sizeB;
            if (posA.x >= maxB.x)
                return false;
            if (posA.y >= maxB.y)
                return false;
            if (maxA.x < posB.x)
                return false;
            if (maxA.y < posB.y)
                return false;

            return true;
        }

        foreach (Rect rect in rects)
        {
            // Set position by looking at all processed rectangles
            for (int i = 0; i < processedRectangles; i++)
            {
                // Resolve position either up or right
                Rect otherRect = rects[i];
                if (otherRect == rect)
                    continue;

                
                bool right = bounds.y >= bounds.x;
                
            }

            bounds.x = Mathf.Max(bounds.x, rect.Max.x);
            bounds.y = Mathf.Max(bounds.y, rect.Max.y);

            processedRectangles++;
        }
    }

    public class Rect
    {
        public Vector2Int Max => position + size;
        public Vector2Int size;
        public Vector2Int position;
        public int index = -1;
    }
}
