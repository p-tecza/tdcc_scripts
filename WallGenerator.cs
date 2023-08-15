using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class WallGenerator
{

    private static HashSet<Vector2Int> allWallTiles = new HashSet<Vector2Int>();

    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        allWallTiles.UnionWith(basicWallPositions);
        foreach (var position in basicWallPositions)
        {
            if (basicWallPositions.Contains(new Vector2Int(position.x, position.y + 1)))
            {
                tilemapVisualizer.PaintSingleSideWall(position);
            }
            else
            {
                tilemapVisualizer.PaintSingleTopWall(position);
            }
        }
    }


    public static HashSet<Vector2Int> getAllWallTiles()
    {
        return allWallTiles;
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }
        return wallPositions;
    }




}

