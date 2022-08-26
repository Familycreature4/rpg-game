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
    public TileTransform TileTransform => GetComponent<TileTransform>();
    public new string name;
    public string partyName;
    float moveDelay = 0.125f;  // Amount of time in seconds between movements
    float nextMoveTime = 0;

    private void Start()
    {
        MoveToWalkableSpace();
        transform.position = World.WorldCoordToScene(TileTransform.coordinates);
        Party.AddToParty(partyName, this);
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
            }
        }

        // Move the gameobject to the world coordinates
        Vector3 targetPosition = (Vector3)(TileTransform.coordinates + Vector3.one / 2.0f) * World.tileSize;
        Vector3 position = Vector3.MoveTowards(transform.position, targetPosition, moveDelay);
        transform.position = position;
    }
    /// <summary>
    /// Attempts to move the pawn in the world
    /// </summary>
    /// <param name="displacement">Offset in tiles to displace the pawn by</param>
    void Move(Vector3Int displacement)
    {
        if (displacement == Vector3Int.zero)
            return;

        Debug.DrawRay(transform.position, displacement, Color.yellow, moveDelay);
        Vector3Int targetCoordinates = TileTransform.coordinates + displacement;
        // Restrict to map bounds
        targetCoordinates.x = Mathf.Clamp(targetCoordinates.x, 0, World.instance.mapWidth - 1);
        targetCoordinates.y = Mathf.Clamp(targetCoordinates.y, 0, World.instance.mapHeight - 1);
        targetCoordinates.z = Mathf.Clamp(targetCoordinates.z, 0, World.instance.mapLength - 1);

        // To do: Give tiles flags (IE walkable, solid) instead
        // Test whether the coordinates are walkable
        if (CanStandHere(targetCoordinates))
        {
            TileTransform.coordinates = targetCoordinates;
            nextMoveTime = Time.time + moveDelay;
        }
    }
    /// <summary>
    /// Teleports the pawn to a walkable tile
    /// </summary>
    void MoveToWalkableSpace()
    {
        if (World.instance.MapReady == false)
            return;

        if (TileTools.GetClosestCoord(TileTransform.coordinates, delegate(Vector3Int c) { return CanStandHere(c); }, out Vector3Int coords))
            TileTransform.coordinates = coords;
    }
    public bool CanStandHere(Vector3Int coord)
    {
        return TileTools.Overlap(coord, TileTransform.size) && World.instance.GetTile(coord + Vector3Int.down).IsSolid;
    }
}
