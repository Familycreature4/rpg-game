using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A character which exists in the world
/// </summary>
public class Pawn : MonoBehaviour
{
    // THIS HAS BEEN CERTIFIED BY THE CREATURE
    // CREATURE CODE QUALITY STAMP OF APPROVAL
    bool CanMove 
    {
        get 
        { 
            return Time.time >= nextMoveTime;
        }
    }
    public Vector3Int worldCoordinates;
    float moveDelay = 0.125f;  // Amount of time in seconds between movements
    float nextMoveTime = 0;

    private void Start()
    {
        MoveToWalkableSpace();
    }

    private void Update()
    {
        // Handle movement logic

        if (CanMove)
        {
            // Get input
            Vector3 input = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                0f,
                Input.GetAxisRaw("Vertical")
            );
            // TO DO: Transform input based on camera location/rotation

            // Limit moves
            Vector3Int move = Vector3Int.RoundToInt(input);
            if (move.magnitude > 0)
            {
                Move(move);
                nextMoveTime = Time.time + moveDelay;
            }
        }

        // Move the gameobject to the world coordinates
        Vector3 targetPosition = (Vector3)worldCoordinates * World.tileSize;
        Vector3 position = Vector3.MoveTowards(transform.position, targetPosition, moveDelay);
        transform.position = position;
    }
    /// <summary>
    /// Attempts to move the pawn in the world
    /// </summary>
    /// <param name="displacement">Offset in tiles to displace the pawn by</param>
    void Move(Vector3Int displacement)
    {
        Debug.DrawRay(transform.position, displacement, Color.yellow, moveDelay);
        // TO DO: Check if pawn can move to new location (Wall is present, no floor?)
        Vector3Int targetCoordinates = worldCoordinates + displacement;
        // Restrict to map bounds
        targetCoordinates.x = Mathf.Clamp(targetCoordinates.x, 0, World.instance.mapWidth - 1);
        targetCoordinates.z = Mathf.Clamp(targetCoordinates.z, 0, World.instance.mapHeight - 1);

        char tile = World.instance.GetTile(targetCoordinates.x, targetCoordinates.z);

        // To do: Give tiles flags (IE walkable, solid) instead
        if (tile == 'X')  // If this tile is walkable
        {
            // Move the pawn
            worldCoordinates = targetCoordinates;
        }
    }
    /// <summary>
    /// Teleports the pawn to a walkable tile
    /// </summary>
    void MoveToWalkableSpace()
    {
        if (World.instance.MapReady == false)
            return;

        // To do: Get the closest current tile instead

        for (int x = 0; x < World.instance.mapWidth; x++)
        {
            for (int z = 0; z < World.instance.mapHeight; z++)
            {
                char tile = World.instance.GetTile(x, z);
                if (tile == 'X')
                {
                    worldCoordinates = new Vector3Int(x, 0, z);
                    return;
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((Vector3)worldCoordinates * World.tileSize, Vector3.one * World.tileSize);
    }
}
