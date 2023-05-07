


using System.Collections.Generic;
using UnityEngine;

public class Room
{

    public HashSet<Vector2Int> FloorTiles { get; set; }
    public HashSet<Vector2Int> WallTiles { get; set; }
    public HashSet<GameObject> RoomObjects { get; set; }
    public int Id { get; set; }
    public List<int> connections { get; set; }
    public Vector2Int entrance { get; set; }
    public Vector2Int exit {  get; set; }
}