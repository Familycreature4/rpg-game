using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    // The 'static' modifier will assign the given property/field/method to the class TYPE instead of a given instance of the class
    // Any property/field/method which is static AND public is accessible from any part of the game (IE global access)
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/static
    public static World instance;  // Singleton pattern
    public static float tileSize = 1.0f;  // Size of a tile in the scene
    #region Map Data
    // This is a temporary solution for storing the level
    // Eventually we'll extend to layers for multiple floors

    // 16 x 16 level
    // # - Wall
    //   - Pit
    // X - Walkable
    public bool MapReady => mapData.Length == mapWidth * mapHeight;
    public int mapWidth = 16;
    public int mapHeight = 16;
    string mapData =
        "################" +
        "#      XXX     #" +
        "#       X      #" +
        "#  XXXX X      #" +
        "#     X XXXXX  #" +
        "#XXX  X##   X  #" +
        "#X X  X##XXXXX #" +
        "#X XXXX##  X X #" +
        "#X    XXXXXX X #" +
        "#XXXXXXXX    X #" +
        "# X ######## X #" +
        "# X   XXXXXXXX #" +
        "# X   XXXX     #" +
        "# XXXXXXXX     #" +
        "#     XXXX     #" +
        "################";
    #endregion
    private void Awake()
    {
        if (instance == null)
            instance = this;

        GenerateMap();
    }
    /// <summary>
    /// Returns the tile at coordinates x, y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public char GetTile(int x, int y)
    {
        // To do: Ensure coordinates are within map range
        return mapData[y * mapWidth + x];
    }
    void GenerateMap()
    {
        // Iterate over map data
        // Create a cube if the tile is not empty

        // To do: Generate a mesh instead of creating gameobjects for every tile => Very inefficient

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                char c = GetTile(x, y);

                Vector3Int coordinates = new Vector3Int(x, 0, y);

                if (c == '#')  // Wall
                {
                    coordinates.y = 0;
                }
                else if (c == 'X')  // Floor
                {
                    coordinates.y = -1;
                }

                if (c != ' ')  // If our tile is NOT empty, create a cube
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = coordinates.ToString();
                    cube.transform.position = (Vector3)coordinates * tileSize;
                    cube.GetComponent<MeshRenderer>().sharedMaterial = Resources.Load<Material>("Wall");
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                char c = GetTile(x, y);

                Vector3Int coordinates = new Vector3Int(x, 0, y);

                if (c == 'X')
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireCube((Vector3)coordinates * tileSize, Vector3.one * tileSize);
                }
            }
        }
    }
}
