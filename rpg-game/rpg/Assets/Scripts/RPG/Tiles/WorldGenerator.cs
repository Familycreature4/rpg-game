using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public static class WorldGenerator
    {
        public static void GenerateMap(Chunk chunk)
        {
            Texture2D mapData = Resources.Load<Texture2D>($"Map/{World.instance.mapName}");

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

                    if (color.r == 0)
                    {
                        // Empty, do nothing
                    }
                    else if (color.r == 145)
                    {
                        // Path, create tile below
                        for (int y = 0; y < Chunk.size; y++)
                        {
                            if (chunk.coords.y * Chunk.size + y <= 4)
                            {
                                chunk.tiles[Chunk.FlattenIndex(x, y, z)] = new Tile("Slate Roof");
                            }
                        }
                            
                    }
                    else if (color.r == 255)
                    {
                        // Wall, create tile above and below
                        for (int y = 0; y < Chunk.size; y++)
                        {
                            if (chunk.coords.y * Chunk.size + y <= 8)
                            {
                                chunk.tiles[Chunk.FlattenIndex(x, y, z)] = new Tile("Brick 2");
                            }
                        }
                    }
                }
            }
        }
    }
}