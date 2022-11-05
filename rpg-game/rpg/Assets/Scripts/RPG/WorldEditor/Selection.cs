using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Editor.Selections
{
    /// <summary>
    /// Defines a volume for an operation
    /// </summary>
    public abstract class Selection
    {
        public abstract IEnumerable<Vector3Int> GetCoords { get; }
    }

    public class Box : Selection
    {
        public Box()
        {

        }
        public BoundsInt bounds;
        public override IEnumerable<Vector3Int> GetCoords
        {
            get
            {
                foreach (Vector3Int c in bounds.allPositionsWithin)
                    yield return c;
            }
        }
    }

    public class Sphere : Selection
    {
        public int radius;
        public Vector3Int center;
        public override IEnumerable<Vector3Int> GetCoords
        {
            get
            {
                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {
                        for (int z = -radius; z <= radius; z++)
                        {
                            Vector3 disp = new Vector3(x, y, z);
                            if (disp.sqrMagnitude <= Mathf.Pow(radius, 2))
                            {
                                yield return center + new Vector3Int(x, y, z);
                            }
                        }
                    }
                }
            }
        }
    }
}


