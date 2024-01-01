


using System.Collections.Generic;
using UnityEngine;

public class Room
{

    
    public HashSet<Vector2Int> FloorTiles { get; set; }
    public HashSet<Vector2Int> WallTiles { get; set; }
    public List<GameObject> droppedItemsInRoom { get; set; }
    public List<GameObject> enemies { get; set; }
    public int Id { get; set; }
    public List<int> connections { get; set; }
    public List<Teleport> teleports { get; set; }
    public List<GameObject> teleportObjects { get; set; }
    public Teleport entrance { get; set; }
    public Teleport exit {  get; set; }
    public bool isComplex { get; set; }

    public Room(bool isComplex)
    {
        this.FloorTiles = new HashSet<Vector2Int>();
        this.WallTiles = new HashSet<Vector2Int>();
        this.droppedItemsInRoom = new List<GameObject>();
        this.enemies = new List<GameObject>();
        this.connections = new List<int>();
        this.teleports = new List<Teleport>();
        this.teleportObjects = new List<GameObject>();
        this.isComplex = isComplex; 
    }


}