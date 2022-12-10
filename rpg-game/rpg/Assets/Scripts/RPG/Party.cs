using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json.Linq;
namespace RPG
{
    /// <summary>
    /// An alliance of pawns
    /// </summary>
    public class Party
    {
        public Party(string name)
        {
            this.name = name;
        }
        public Pawn Leader => pawns.Count > 0 ? pawns[0] : null;
        public bool CanMove => Leader.CanMove;
        public int Count => pawns.Count;
        public bool AllDead => pawns.All((Pawn pawn) => { return pawn.IsAlive == false; });
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
        public IEnumerable<Pawn> Pawns
        {
            get
            {
                foreach (Pawn pawn in pawns)
                    yield return pawn;
            }
        }
        List<Pawn> pawns = new List<Pawn>();
        public System.Action<Party> onMove;
        public string name;
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
        public void AddPawn(Pawn pawn)
        {
            pawn.party = this;
            pawns.Add(pawn);
        }
        /// <summary>
        /// Converts a coordinate in local party space to world space
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public Vector3Int TransformFormationPosition(Vector3Int local)
        {
            Vector3 pos = Quaternion.AngleAxis(FormationRotation, Vector3.up) * local;
            pos += Leader.TileTransform.coordinates;
            return Vector3Int.RoundToInt(pos);
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
        public int IndexOf(Pawn pawn) => pawns.IndexOf(pawn);
        public virtual void Update()
        {
            
        }
        public bool Contains(Pawn p) => pawns.Contains(p);
        public bool Move(Vector3Int displacement)
        {
            if (Director.Current.Battle != null)
                return false;

            if (Leader.Move(displacement))
            {
                EventManager.onPartyMove?.Invoke(this);
                // Reset other party members' timers
                foreach (Pawn otherPawn in pawns)
                {
                    if (otherPawn != Leader)
                    {
                        otherPawn.ResetMoveTimer();
                    }
                }
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
        public static Party FromJson(string path, Vector3Int position = default)
        {
            if (System.IO.File.Exists(path) == false)
                return null;

            Party party = new Party("Player party");
            JObject json = JObject.Parse(System.IO.File.ReadAllText(path));
            GameObject prefab = Resources.Load<GameObject>("Prefabs/GAY CUBE PAWN");

            foreach (JObject jPawn in json["pawns"])
            {
                GameObject pawnObject = GameObject.Instantiate(prefab, position, Quaternion.identity);
                Pawn pawn = pawnObject.GetComponent<Pawn>();
                pawn.name = jPawn["name"].ToString();

                pawn.stats = Stats.FromJson(jPawn["stats"] as JObject);

                party.AddPawn(pawn);

                pawn.image = Resources.Load<Sprite>(jPawn["picture"].ToString());
            }

            return party;
        }
        public static Party Spawn(Vector3Int position, bool player = false)
        {
            // Manuall initialize
            string name = player ? "Player Party" : "Enemy Party";
            Party party = new Party(name);
            GameObject prefab = player ? Resources.Load<GameObject>("Prefabs/GAY CUBE PAWN") : Resources.Load<GameObject>("Prefabs/ENEMY");
            for (int i = 0; i < 5; i++)
            {
                GameObject pawnObject = GameObject.Instantiate(prefab, position, Quaternion.identity);
                Pawn pawn = pawnObject.GetComponent<Pawn>();

                pawn.name = "ENEMY";
                pawn.image = Resources.Load<Sprite>("Sprites/fmbot_avatars/floppa");

                party.AddPawn(pawn);

                pawn.stats = new Stats();
            }

            return party;
        }
    }
}