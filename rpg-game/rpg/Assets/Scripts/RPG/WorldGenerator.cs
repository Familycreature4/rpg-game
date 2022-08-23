using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldGenerator
{
    public static void GenerateMap(World world)
    {
        Texture2D mapData = Resources.Load<Texture2D>("Map/map");
        world.mapWidth = mapData.width;
        world.mapLength = mapData.height;
        world.mapHeight = 2;
        world.tiles = new Tile[world.mapWidth * world.mapLength * world.mapHeight];
        // Iterate over map data
        // Create a cube if the tile is not empty

        // To do: Generate a mesh instead of creating gameobjects for every tile => Very inefficient

        TileData solidTile = Resources.Load<TileData>("Tiles/Solid");
        
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
                    world.tiles[world.FlattenIndex(x, 0, z)] = new Tile(solidTile);
                }
                else if (color.r == 255)
                {
                    // Wall, create tile above and below
                    world.tiles[world.FlattenIndex(x, 0, z)] = new Tile(solidTile);
                    world.tiles[world.FlattenIndex(x, 1, z)] = new Tile(solidTile);
                }
            }
        }
    }
}
