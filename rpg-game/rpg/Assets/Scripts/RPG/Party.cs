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
        static Dictionary<string, Party> parties = new Dictionary<string, Party>();
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
        public List<Pawn> pawns = new List<Pawn>();
        // Formations are relative to the pawn Leader (The first pawn in the pawn list)
        public Vector3Int[] formationLocalPositions = {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(-1, 0, -1),
            new Vector3Int(-1, 0, -2),
            new Vector3Int(1, 0, -2),
        };
        public float formationRotation = 0.0f;  // Rotates party around leader
        public Vector3Int TransformFormationPosition(Vector3Int local)
        {
            Vector3 pos = Quaternion.AngleAxis(Mathf.Round(formationRotation / 90.0f) * 90.0f, Vector3.up) * local;
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
                center += pawn.transform.position;
            }

            center /= pawns.Count;

            return center;
        }
        public void Move(Vector3Int displacement)
        {
            Leader?.Move(displacement);
        }
    }
}