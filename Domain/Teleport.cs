


using System.Collections.Generic;
using UnityEngine;

public class Teleport
{
    public Vector2Int teleportTo { get; set; }
    public Vector2Int teleportFrom { get; set; }
    public int teleportToRoomId { get; set; }
    public RelativeDirection relativeLocation { get; set; }

}