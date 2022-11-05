using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public static class WorldGenerator
    {
        readonly static Color32 dirt = new Color32(111, 92, 81, 255);
        readonly static Color32 brick = new Color32(166, 166, 166, 255);
        readonly static Color32 foliage = new Color32(8, 90, 0, 255);
        readonly static Color32 playerParty = new Color32(0, 255, 0, 255);
        readonly static Color32 enemyParty = new Color32(255, 0, 0, 255);
        readonly static Vector2Int[] neighbors = new Vector2Int[4]
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };
        readonly static string[] trees = new string[]
        {
            "Tree Large 01",
            "Tree Large 02",
            "Tree Large 03",
            //"Tree Large 04",
            //"Tree Medium 01",
            //"Tree Medium 02",
            //"Tree Small 01"
        };
        public static void GenerateMap(Chunk chunk)
        {
            Texture2D mapData = Resources.Load<Texture2D>($"Map/{"DEAD"}");
            int brickHeight = 10;
            int foliageHeight = 4;

            for (int i = 0; i < Chunk.sizeCubed; i++)
                chunk.tiles[i] = Tile.Air;

            for (int x = 0; x < Chunk.size; x++)
            {
                for (int z = 0; z < Chunk.size; z++)
                {
                    // Sample image
                    // Check if coords are in image bounds
                    Vector3Int worldCoords = chunk.coords * Chunk.size + new Vector3Int(x, 0, z);
                    if (worldCoords.x < 0 || worldCoords.x >= mapData.width || worldCoords.z < 0 || worldCoords.z >= mapData.height)
                        continue;

                    Color32 color = mapData.GetPixel(worldCoords.x, worldCoords.z);

                    for (int y = 0; y < Chunk.size; y++)
                    {
                        if (chunk.coords.y * Chunk.size + y <= 0)
                        {
                            chunk.tiles[Chunk.FlattenIndex(x, y, z)] = new Tile("Grass 1");
                        }
                    }

                    if (color.Equals(foliage))
                    {
                        // Check if all neighbors are air
                        bool allAir = true;
                        if (worldCoords.x == 0 || worldCoords.x == mapData.width - 1 || worldCoords.z == 0 || worldCoords.z == mapData.height - 1)
                        {
                            allAir = false;
                        }
                        else
                        {
                            foreach (Vector2Int neighbor in neighbors)
                            {
                                if (mapData.GetPixel(neighbor.x + worldCoords.x, neighbor.y + worldCoords.z).Equals(foliage))
                                {
                                    allAir = false;
                                    break;
                                }
                            }
                        }
                        
                        if (allAir)
                        {
                            if (chunk.coords.y == 0)
                            {
                                int index = Random.Range(0, trees.Length);
                                World.Current.SpawnProp($"Foliage/{trees[index]}", worldCoords + Vector3Int.up, Quaternion.identity);
                            }
                        }
                        else
                        {
                            for (int y = 0; y < Chunk.size; y++)
                            {
                                if (chunk.coords.y * Chunk.size + y <= foliageHeight)
                                {
                                    chunk.tiles[Chunk.FlattenIndex(x, y, z)] = new Tile("Hedge");
                                }
                            }
                        }

                        
                    }
                    else if (color.Equals(brick))
                    {
                        for (int y = 0; y < Chunk.size; y++)
                        {
                            if (chunk.coords.y * Chunk.size + y <= brickHeight)
                            {
                                chunk.tiles[Chunk.FlattenIndex(x, y, z)] = new Tile("Brick 2");
                            }
                        }
                    }

                    if (chunk.coords.y == 0)
                    {
                        if (color.Equals(enemyParty))
                        {
                            Director.Current.SpawnParty(worldCoords + Vector3Int.up, true);
                        }
                        else if (color.Equals(playerParty))
                        {
                            Director.Current.SpawnParty(worldCoords + Vector3Int.up, false);
                        }
                    }
                }
            }
        }
    }
}