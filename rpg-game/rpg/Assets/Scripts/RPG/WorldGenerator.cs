using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG
{
    public static class WorldGenerator
    {
        public static void GenerateMap(World world)
        {
            Texture2D mapData = Resources.Load<Texture2D>($"Map/{world.mapName}");
            world.mapWidth = mapData.width;
            world.mapLength = mapData.height;
            world.mapHeight = 10;
            world.tiles = new Tile[world.mapWidth * world.mapLength * world.mapHeight];

            for (int i = 0; i < world.mapVolume; i++)
            {
                world.tiles[i] = Tile.Air;
            }


            for (int x = 0; x < world.mapWidth; x++)
            {
                for (int z = 0; z < world.mapLength; z++)
                {
                    // Sample image
                    Color32 color = mapData.GetPixel(x, z);

                    if (color.r == 0)
                    {
                        // Empty, do nothing
                    }
                    else if (color.r == 145)
                    {
                        // Path, create tile below
                        for (int y = 4; y >= 0; y--)
                            world.tiles[world.FlattenIndex(x, y, z)] = new Tile("Clay");
                    }
                    else if (color.r == 255)
                    {
                        // Wall, create tile above and below
                        for (int y = 0; y < world.mapHeight; y++)
                            world.tiles[world.FlattenIndex(x, y, z)] = new Tile("Brick 2");
                    }
                }
            }
        }
    }
}