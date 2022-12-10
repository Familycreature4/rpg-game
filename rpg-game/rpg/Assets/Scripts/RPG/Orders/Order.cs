using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace RPG.Orders
{
    /// <summary>
    /// Represents a task a pawn will do
    /// </summary>
    public abstract class Order
    {
        public Order(Pawn pawn, int priority = 0)
        {
            this.pawn = pawn;
            isComplete = false;
            this.priority = priority;
        }
        public int Priority => priority;
        public bool IsComplete { get => isComplete; set { isComplete = value; } }
        protected Pawn pawn;
        protected bool isComplete;
        protected int priority;

        public abstract void Think();
    }

    public class MoveTo : Order
    {
        public MoveTo(Pawn pawn, Func<Vector3Int> positionGetter, bool completeOnReach = true, int priority = 0) : base(pawn, priority)
        {
            this.positionGetter = positionGetter;
            this.completeOnReach = completeOnReach;
        }
        public MoveTo(Pawn pawn, Vector3Int coords, bool completeOnReach = true) : base(pawn)
        {
            this.completeOnReach = completeOnReach;
            this.positionGetter = () => { return coords; };
        }
        public bool AtPosition => positionGetter() == pawn.Coordinates;
        public System.Func<Vector3Int> PositionGetter { get => positionGetter; set { positionGetter = value; } }
        bool completeOnReach = false;
        System.Func<Vector3Int> positionGetter;
        public override void Think()
        {
            if (positionGetter == null)
            {
                isComplete = true;
                return;
            }

            Vector3Int targetCoords = positionGetter();

            if (pawn.Coordinates == targetCoords)
            {
                if (completeOnReach)
                    isComplete = true;
                return;
            }

            if (pawn.CanMove)
            {
                // Path find to target coords
                if (TileTools.GetPath(pawn.Coordinates,
                                        targetCoords,
                                        out List<Vector3Int> path,
                                        delegate (Vector3Int c) { return pawn.CanStandHere(c); },
                                        null,
                                        (int)Vector3.Distance(pawn.Coordinates, targetCoords) * 10 + 10))
                {
                    Vector3Int targetPos = path[0];
                    if (path.Count > 1)
                        targetPos = path[1];

                    pawn.Move(targetPos - pawn.Coordinates);

                    for (int a = 0; a < path.Count - 1; a++)
                    {
                        Debug.DrawLine(path[a] + Vector3.one / 2, path[a + 1] + Vector3.one / 2, Color.white);
                    }
                    for (int a = 0; a < path.Count; a++)
                    {
                        Debug.DrawRay(path[a] + Vector3.one / 2, Vector3.up, Color.red);
                    }
                }
            }
        }
    }

}
