


using System.Collections.Generic;
using UnityEngine;

public class Room
{

    
    public HashSet<Vector2Int> FloorTiles { get; set; }
    public HashSet<Vector2Int> WallTiles { get; set; }
    public HashSet<GameObject> RoomObjects { get; set; }
    public List<GameObject> enemies { get; set; }
    public int Id { get; set; }
    public List<int> connections { get; set; }
    public Teleport entrance { get; set; }
    public Teleport exit {  get; set; }

    public Room()
    {
        this.FloorTiles = new HashSet<Vector2Int>();
        this.WallTiles = new HashSet<Vector2Int>();
        this.RoomObjects = new HashSet<GameObject>();
        this.enemies = new List<GameObject>();
        this.connections = new List<int>();
    }


}