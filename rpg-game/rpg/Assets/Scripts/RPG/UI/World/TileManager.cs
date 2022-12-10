using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.World
{
    public class TileManager
    {
        public TileManager()
        {
            tileObjects = new List<GameObject>();

            EventManager.OnSelect += OnSelect;

            EventManager.onAttackerTurnStart += OnTurnBegin;
            EventManager.onBattleEnd += OnBattleEnd;
        }
        List<GameObject> tileObjects;
        public void DisplayTiles(Pawn pawn, float maxDistance = 10.0f)
        {
            DestroyTiles();

            Vector3Int pawnCoords = pawn.Coordinates;

            Queue<Vector3Int> openCoords = new Queue<Vector3Int>();
            HashSet<Vector3Int> closedCoords = new HashSet<Vector3Int>();

            openCoords.Enqueue(pawn.Coordinates);

            while (openCoords.Count > 0)
            {
                Vector3Int currentCoord = openCoords.Dequeue();

                foreach (Vector3Int direction in TileTools.directionsAll)
                {
                    Vector3Int newCoord = currentCoord + direction;
                    if (closedCoords.Contains(newCoord))
                        continue;

                    closedCoords.Add(newCoord);

                    float sqrDistance = (pawnCoords - newCoord).sqrMagnitude;
                    if (sqrDistance <= maxDistance * maxDistance && pawn.CanStandHere(newCoord))
                    {
                        if (MoveHelper.TryMove(currentCoord, newCoord, pawn.TileTransform.size, out Vector3Int correctCoords, pawn.TileTransform, pawn.party))
                        {
                            openCoords.Enqueue(correctCoords);
                            // Create tile
                            GameObject tile = CreateTileGameObject(correctCoords);
                            tileObjects.Add(tile);
                        }
                    }
                }
            }
        }
        public void DestroyTiles()
        {
            foreach (GameObject go in tileObjects)
            {
                GameObject.Destroy(go);
            }

            tileObjects.Clear();
        }
        void OnPlayerSelectPawn(RPGPlayer player, Pawn pawn)
        {
            
        }
        void OnPlayerDeselectPawn(RPGPlayer player, Pawn pawn)
        {
            DestroyTiles();
        }
        void OnBattleEnd(RPG.Battle.Battle battle)
        {
            DestroyTiles();
        }
        void OnTurnBegin(RPG.Battle.Battle battle, RPG.Battle.Attackers.Attacker attacker)
        {
            DestroyTiles();
        }
        void OnSelect(Events.SelectArgs args)
        {
            if (args.IsPawn && Director.Current.Battle != null && args.Pawn.party.IsPlayer)
                DisplayTiles(args.Pawn, args.Pawn.battleMoveDistance);
            else
                DestroyTiles();
        }
        public static GameObject CreateTileGameObject(Vector3Int coords = default)
        {
            GameObject tile = GameObject.Instantiate(Resources.Load<GameObject>("UI/World/Tile"), coords + new Vector3(0.5f, 0.01f, 0.5f), Quaternion.Euler(90, 0, 0));
            tile.GetComponent<Canvas>().worldCamera = Player.Current.Camera;
            return tile;
        }
    }
}
