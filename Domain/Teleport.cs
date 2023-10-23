


using System.Collections.Generic;
using UnityEngine;

public class Teleport
{
    public Vector2Int teleportTo { get; set; }
    public Vector2Int teleportFrom { get; set; }
    public int teleportToRoomId { get; set; }
    public RelativeDirection relativeLocation { get; set; }

    public Teleport() { }
    public Teleport(Vector2Int teleportTo, Vector2Int teleportFrom, int teleportToRoomId, RelativeDirection relativeDirection)
    {
        this.teleportTo = teleportTo;
        this.teleportFrom = teleportFrom;
        this.teleportToRoomId = teleportToRoomId;
        this.relativeLocation = relativeDirection;
    }
    
}