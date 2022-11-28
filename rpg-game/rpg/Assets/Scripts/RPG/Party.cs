using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    /// <summary>
    /// An alliance of pawns
    /// </summary>
    public class Party
    {
        public static Dictionary<string, Party> parties = new Dictionary<string, Party>();
        public static bool AddToParty(string name, Pawn pawn)
        {
            if (parties.TryGetValue(name, out Party party) == false)
            {
                // Create one
                party = new Party();
                parties.Add(name, party);
            }

            if (party.pawns.Contains(pawn) == false)
            {
                pawn.party = party;
                party.pawns.Add(pawn);
                return true;
            }

            return false;
        }
        public static Party GetParty(string name)
        {
            if (parties.TryGetValue(name, out Party p))
                return p;

            return null;
        }
        public Pawn Leader => pawns.Count > 0 ? pawns[0] : null;
        public bool CanMove => Leader.CanMove;
        public int Count => pawns.Count;
        public bool IsPlayer
        {
            get
            {
                if (Player.Current != null && Player.Current is RPGPlayer rpgPlayer)
                {
                    return rpgPlayer.Party == this;
                }

                return false;
            }
        }
        public List<Pawn> pawns = new List<Pawn>();
        public System.Action<Party> onMove;
        // Formations are relative to the pawn Leader (The first pawn in the pawn list)
        public Vector3Int[] formationLocalPositions = {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, -2),
            new Vector3Int(1, 0, -2),
        };
        public float FormationRotation  // Rotates party around leader
        {  
            get { return formationRotation % 360.0f; }
            set { formationRotation = value; }
        }
        float formationRotation = 0.0f;
        /// <summary>
        /// Converts a coordinate in local party space to world space
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public Vector3Int TransformFormationPosition(Vector3Int local)
        {
            Vector3 pos = Quaternion.AngleAxis(FormationRotation, Vector3.up) * local;
            pos += Leader.TileTransform.coordinates;
            return Vector3Int.FloorToInt(pos);
        }
        public Vector3 GetCenter()
        {
            if (pawns.Count == 0)
                return Vector3.zero;

            Vector3 center = Vector3.zero;
            foreach (Pawn pawn in pawns)
            {
                center += pawn.TileTransform.Bounds.Center + Vector3.one / 2;
            }

            center /= pawns.Count;

            return center;
        }
        public virtual void Update()
        {
            
        }
        public bool Contains(Pawn p) => pawns.Contains(p);
        public bool Move(Vector3Int displacement)
        {
            if (Leader.Move(displacement))
            {
                onMove?.Invoke(this);
                return true;
            }

            return false;
        }
        public UnityEngine.BoundsInt GetBounds()
        {
            UnityEngine.Bounds bounds = new UnityEngine.Bounds(GetCenter(), Vector3.zero);

            foreach (Pawn pawn in pawns)
            {
                TileTransform tileTransform = pawn.TileTransform;
                Vector3 pawnCenter = tileTransform.coordinates + ((Vector3)tileTransform.size / 2.0f);
                UnityEngine.Bounds pawnBounds = new UnityEngine.Bounds(pawnCenter, tileTransform.size);
                bounds.Encapsulate(pawnBounds);
            }

            return new BoundsInt { min = Vector3Int.FloorToInt(bounds.min), max = Vector3Int.CeilToInt(bounds.max) };
        }
    }
}