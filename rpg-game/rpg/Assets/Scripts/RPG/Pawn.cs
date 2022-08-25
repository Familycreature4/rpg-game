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
    public Vector3Int size = new Vector3Int(1, 2, 1);  // Size in tiles
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
        Vector3 targetPosition = (Vector3)(worldCoordinates + Vector3.one / 2.0f) * World.tileSize;
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
        Vector3Int targetCoordinates = worldCoordinates + displacement;
        // Restrict to map bounds
        targetCoordinates.x = Mathf.Clamp(targetCoordinates.x, 0, World.instance.mapWidth - 1);
        targetCoordinates.y = Mathf.Clamp(targetCoordinates.y, 0, World.instance.mapHeight - 1);
        targetCoordinates.z = Mathf.Clamp(targetCoordinates.z, 0, World.instance.mapLength - 1);

        // To do: Give tiles flags (IE walkable, solid) instead
        // Test whether the coordinates are walkable
        if (CanStandHere(targetCoordinates))
        {
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
            for (int z = 0; z < World.instance.mapLength; z++)
            {
                if (CanStandHere(new Vector3Int(x, 1, z)))
                {
                    worldCoordinates = new Vector3Int(x, 1, z);
                    return;
                }
            }
        }
    }
    public bool CanStandHere(Vector3Int coord)
    {
        return Overlap(coord, size) && World.instance.GetTile(coord + Vector3Int.down).IsSolid;
    }
    /// <summary>
    /// Returns whether a pawn at coords and size can fit here
    /// </summary>
    /// <param name="coords"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public bool Overlap(Vector3Int coords, Vector3Int size)
    {
        for (int x = -size.x / 2; x < size.x / 2 + 1; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = -size.z / 2; z < size.z / 2 + 1; z++)
                {
                    Vector3Int worldCoord = coords + new Vector3Int(x, y, z);
                    if (World.instance.GetTile(worldCoord).IsSolid)
                        return false;
                }
            }
        }

        return true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube((Vector3)(worldCoordinates + new Vector3(0.5f, size.y / 2.0f, 0.5f)) * World.tileSize, (Vector3)size * World.tileSize);
    }
}
