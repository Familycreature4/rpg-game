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
    float tileSize = 1.0f;  // Move to a different class
    public Vector3Int worldCoordinates;
    float moveDelay = 0.125f;  // Amount of time in seconds between movements
    float nextMoveTime = 0;

    private void Start()
    {
        worldCoordinates = Vector3Int.FloorToInt(transform.position);
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
        Vector3 targetPosition = (Vector3)worldCoordinates * tileSize;
        Vector3 position = Vector3.MoveTowards(transform.position, targetPosition, moveDelay);
        transform.position = position;
    }
    /// <summary>
    /// Attempts to move the pawn in the world
    /// </summary>
    /// <param name="displacement">Offset in tiles to displace the pawn by</param>
    void Move(Vector3Int displacement)
    {
        // TO DO: Check if pawn can move to new location (Wall is present, no floor?)

        worldCoordinates += displacement;
    }
}
