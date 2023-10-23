using System.Collections.Generic;
using UnityEngine;

public class TeleportOrientationHelper
{

    private static readonly List<RelativeDirection> directionsToConsider = new List<RelativeDirection>() {
        RelativeDirection.South, RelativeDirection.North, RelativeDirection.East, RelativeDirection.North
    };

    public  static (Teleport, Teleport) DefineLocationOfTeleports(GraphConnection gc, GridAlgorithm.GridGraph gg, Room parentRoom, Room childRoom)
    {
        Teleport parentTeleport = new Teleport();
        Teleport childTeleport = new Teleport();
        RelativeDirection locationOfParentTeleport = DeterminePassageDirection(gc, gg);
        RelativeDirection locationOfChildTeleport = OppositeDirection(locationOfParentTeleport);
        Dictionary<RelativeDirection, List<Vector2Int>> parentTilesGroupedByLocation = GroupRoomTilesByLocation(parentRoom);
        Dictionary<RelativeDirection, List<Vector2Int>> childTilesGroupedByLocation = GroupRoomTilesByLocation(childRoom);
        parentTeleport.teleportFrom = SelectTileForTeleportFrom(parentTilesGroupedByLocation[locationOfParentTeleport], locationOfParentTeleport);
        childTeleport.teleportFrom = SelectTileForTeleportFrom(childTilesGroupedByLocation[locationOfChildTeleport], locationOfChildTeleport);
        parentTeleport.teleportTo = FindTeleportToLocation(childTeleport.teleportFrom, childRoom);
        childTeleport.teleportTo = FindTeleportToLocation(parentTeleport.teleportFrom, parentRoom);
        parentTeleport.relativeLocation = locationOfParentTeleport;
        childTeleport.relativeLocation = locationOfChildTeleport;
        parentTeleport.teleportToRoomId = childRoom.Id;
        childTeleport.teleportToRoomId = parentRoom.Id;

        return (parentTeleport, childTeleport);
    }

    private static Vector2Int FindTeleportToLocation(Vector2Int childTeleportFrom, Room childRoom)
    {
        return FindProperRoomTileAroundTeleportLocation(childTeleportFrom, childRoom.FloorTiles);
    }

    private static Vector2Int SelectTileForTeleportFrom(List<Vector2Int> availableTiles, RelativeDirection relativeLocation)
    {
        int sizeOfTilesCollection = availableTiles.Count;
        if(relativeLocation == RelativeDirection.West || relativeLocation == RelativeDirection.East)
        {
            availableTiles.Sort((a, b) => a.y.CompareTo(b.y));
        }
        else
        {
            availableTiles.Sort((a, b) => a.x.CompareTo(b.x));
        }

        int chosenIndex = availableTiles.Count / 2;
        int randomOffset = UnityEngine.Random.Range(0, availableTiles.Count / 3);
        int randomSign = UnityEngine.Random.Range(0, 2); //0: '-', 1: '+'

        if(randomSign == 0)
        {
            if(chosenIndex - randomOffset > 0)
            {
                chosenIndex = chosenIndex - randomOffset;
            }
            
        }
        else
        {
            if (chosenIndex + randomOffset < availableTiles.Count - 1)
            {
                chosenIndex = chosenIndex + randomOffset;
            }
        }

        return availableTiles[chosenIndex];
    }

    private static Dictionary<RelativeDirection, List<Vector2Int>> GroupRoomTilesByLocation(Room room)
    {
/*        if (room.isComplex)
        {
            return GroupComplexRoomTilesByLocation(room);
        }*/

        HashSet<Vector2Int> tiles = room.WallTiles;

        int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;

        foreach (Vector2Int tile in tiles) 
        {
            if (tile.x < minX) minX = tile.x;
            if (tile.x > maxX) maxX = tile.x;
            if (tile.y < minY) minY = tile.y;
            if (tile.y > maxY) maxY = tile.y;
        }

        List<Vector2Int> southTiles = new List<Vector2Int>();
        List<Vector2Int> northTiles = new List<Vector2Int>();
        List<Vector2Int> eastTiles = new List<Vector2Int>();
        List<Vector2Int> westTiles = new List<Vector2Int>();

        foreach (Vector2Int tile in tiles)
        {
            if(tile.x == minX) eastTiles.Add(tile);
            else if(tile.x == maxX) westTiles.Add(tile);
            else if(tile.y == minY) southTiles.Add(tile);
            else if(tile.y == maxY) northTiles.Add(tile);
        }

        return new Dictionary<RelativeDirection, List<Vector2Int>>()
        {
            { RelativeDirection.South, southTiles },
            { RelativeDirection.North, northTiles },
            { RelativeDirection.East, eastTiles },
            { RelativeDirection.West, westTiles }
        };
    }

    private static Dictionary<RelativeDirection, List<Vector2Int>> GroupComplexRoomTilesByLocation(Room room)
    {



        return null;
    }

    private static RelativeDirection DeterminePassageDirection(GraphConnection gc, GridAlgorithm.GridGraph gg)
    {
        int dim = gg.dimension;
        RelativeDirection finalDirection = RelativeDirection.South;
        if(gc.parentNode + dim == gc.childNode)
        {
            finalDirection = RelativeDirection.South;
        }
        else if (gc.parentNode - dim == gc.childNode)
        {
            finalDirection = RelativeDirection.North;
        }
        else if (gc.parentNode + 1 == gc.childNode)
        {
            finalDirection= RelativeDirection.East;
        }
        else if (gc.parentNode - 1 == gc.childNode)
        {
            finalDirection = RelativeDirection.West;
        }
        return finalDirection;
    }

    private static RelativeDirection OppositeDirection(RelativeDirection relativeDirection)
    {
        if(relativeDirection == RelativeDirection.North)
        {
            return RelativeDirection.South;
        }
        else if (relativeDirection == RelativeDirection.South)
        {
            return RelativeDirection.North;
        }
        else if (relativeDirection == RelativeDirection.East)
        {
            return RelativeDirection.West;
        }
        else if(relativeDirection == RelativeDirection.West)
        {
            return RelativeDirection.East;
        }
        return RelativeDirection.North;
    }

    private static Vector2Int FindProperRoomTileAroundTeleportLocation(Vector2Int teleportLocation, HashSet<Vector2Int> roomFloorTiles)
    {
        Vector2Int newPosition = CalculateProperRoomTileAroundTeleportLocation(roomFloorTiles, teleportLocation, 2);
        if (newPosition == teleportLocation)
        {
            newPosition = CalculateProperRoomTileAroundTeleportLocation(roomFloorTiles, teleportLocation, 1);
        }

        return newPosition;
    }

    private static Vector2Int CalculateProperRoomTileAroundTeleportLocation(HashSet<Vector2Int> roomFloorTiles,
    Vector2Int teleportLocation, int distance)
    {

        Vector2Int leftPosition = new Vector2Int(teleportLocation.x - distance, teleportLocation.y);
        Vector2Int rightPosition = new Vector2Int(teleportLocation.x + distance, teleportLocation.y);
        Vector2Int topPosition = new Vector2Int(teleportLocation.x, teleportLocation.y + distance);
        Vector2Int bottomPosition = new Vector2Int(teleportLocation.x + 1, teleportLocation.y - distance);


        if (roomFloorTiles.Contains(leftPosition))
        {
            return leftPosition;
        }
        else if (roomFloorTiles.Contains(rightPosition))
        {
            return rightPosition;
        }
        else if (roomFloorTiles.Contains(bottomPosition))
        {
            return bottomPosition;
        }
        else if (roomFloorTiles.Contains(topPosition))
        {
            return topPosition;
        }
        return teleportLocation;

    }



}