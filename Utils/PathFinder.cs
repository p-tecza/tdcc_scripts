using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PathFinder
{
    public static (bool, Vector2Int) AreAnyObstaclesInAWay(List<Vector2Int> obstacleTiles, Vector3 currentPos, Vector3 vectorToMove,
        List<Vector2> collisionSensors)
    {

        Vector2Int currentTile = new Vector2Int((int)System.Math.Round(currentPos.x), (int)System.Math.Round(currentPos.y));

        List<Vector2Int> adjacentTiles = new List<Vector2Int>();

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                adjacentTiles.Add(new Vector2Int(currentTile.x + i, currentTile.y + j));
            }
        }

        List<Vector2Int> adjacentWallTiles = new List<Vector2Int>();


        foreach (var tile in adjacentTiles)
        {
            if (obstacleTiles.Contains(tile))
            {
                adjacentWallTiles.Add(tile);
            }
        }

        foreach (Vector2 collisionSensor in collisionSensors)
        {
            Vector2 sensorPos = new Vector2(currentPos.x, currentPos.y) + collisionSensor;
            Vector2 destPos = sensorPos + new Vector2(vectorToMove.x, vectorToMove.y);

            foreach (var tile in adjacentWallTiles)
            {
                if (CheckIfIntersectsInsideTheTile(destPos, tile))
                {
                    return (true, tile);
                }
            }

        }

        return (false, Vector2Int.zero);

    }

    /*    private static float[] CalculateLinesIntersection(Dictionary<String, float> xy, float a, float b)
        {
            float x, y;
            if (xy.Keys.Count == 1 && xy.Keys.Contains("x"))
            {
                y = xy["x"] * a + b;

                return new float[] { xy["x"], y };

            } else if (xy.Keys.Count == 1 && xy.Keys.Contains("y"))
            {
                x = (xy["y"] - b) / a;

                return new float[] { x, xy["y"] };
            }

            return null;
        }

        private static float[] CalculateLineEquation(Vector2 firstPoint, Vector2 secondPoint)
        {
            float yDiff = secondPoint.y - firstPoint.y;
            float aDiff = secondPoint.x - firstPoint.x;

            if(firstPoint.x == secondPoint.x)
            {
                return new float[] { 1, 0, firstPoint.x };
            }
            else if(firstPoint.y == secondPoint.y)
            {
                return new float[] { 0, firstPoint.y };
            }

            float a = yDiff / aDiff;
            float b = firstPoint.y - firstPoint.x * a;
            float[] res = {a,b};
            return res;
        }*/

    private static bool CheckIfIntersectsInsideTheTile(Vector2 intersection, Vector2Int tilePos)
    {

        if (intersection.x >= tilePos.x && intersection.x <= tilePos.x + 1
            && intersection.y >= tilePos.y && intersection.y <= tilePos.y + 1)
        {
            return true;
        }
        return false;
    }

    public static Vector3 FindNewMovementVector(Vector3 currentMovementVector, int nPossibility)
    {
        //
        Vector2 reducedVec = new Vector2(currentMovementVector.x, currentMovementVector.y);
        // Pitagoras
        float a = reducedVec.y;
        float b = reducedVec.x;
        float c = reducedVec.magnitude;

        float radians = (float)Math.Asin(a / c);
        float deg = (float)(radians * 180 / Math.PI);
        float newDeg = deg - nPossibility * 10;
        float newRadians = (float)(newDeg * Math.PI / 180);
        float sinVal = (float)Math.Sin(newRadians);
        float cosVal = (float)Math.Cos(newRadians);
        float newA = sinVal * c;
        float newB = cosVal * c;

        return new Vector3(newB, newA, 0);
    }


    public static Vector3 DetermineOptimalVector(Vector2 vector1, Vector2 vector2, Vector2 vectorToCompare)
    {
        double firstAngle = Math.Abs(AngleBetween(vector1, vectorToCompare));
        double secondAngle = Math.Abs(AngleBetween(vector2, vectorToCompare));

        if (firstAngle < secondAngle)
        {
            return new Vector3(vector1.x, vector1.y, 0);
        }
        else
        {
            return new Vector3(vector2.x, vector2.y, 0);
        }
    }
    public static double AngleBetween(Vector2 vector1, Vector2 vector2)
    {
        double sin = vector1.x * vector2.y - vector2.x * vector1.y;
        double cos = vector1.x * vector2.y + vector1.y * vector2.y;

        return Math.Atan2(sin, cos) * (180 / Math.PI);
    }

}