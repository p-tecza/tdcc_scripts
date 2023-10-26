using System.Collections.Generic;
using System;
using UnityEngine;

public class RoomHelper
{
    public static Vector2Int DetermineRoomCenter(HashSet<Vector2Int> roomTiles)
    {

        int minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = Int32.MinValue, maxY = Int32.MinValue;
        Vector2Int substitatePlacement = new Vector2Int();

        foreach (Vector2Int tile in roomTiles)
        {

            if (tile.x < minX) minX = tile.x;
            else if (tile.x > maxX) maxX = tile.x;

            if (tile.y < minY) minY = tile.y;
            else if (tile.y > maxY) maxY = tile.y;

            substitatePlacement = tile;
        }

        Vector2Int pseudoCenter = new Vector2Int((minX + maxX) / 2, (minY + maxY) / 2);

        if (!roomTiles.Contains(pseudoCenter))
        {
            return substitatePlacement;
        }

        return pseudoCenter;
    }


}