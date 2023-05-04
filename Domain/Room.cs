


using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public HashSet<Vector2Int> FloorTiles { get; set; }
    public HashSet<Vector2Int> RoomTiles { get; set; }

    public HashSet<GameObject> RoomObjects { get; set; }

    public int Id { get; set; }

    public HashSet<int> connections { get; set; }
}