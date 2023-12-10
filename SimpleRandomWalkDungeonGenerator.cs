using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;
    [SerializeField]
    protected bool isSeeded;
    [SerializeField]
    protected int seed;

    public void Start()
    {
        if (this.isSeeded)
        {
            Debug.Log("SEEDED");
            UnityEngine.Random.InitState(this.seed);
        }
        else
        {
            Debug.Log("NOT SEEDED");
            UnityEngine.Random.InitState(UnityEngine.Random.Range(0,int.MaxValue));
        }
        
    }

    protected override void RunProceduralGeneration(int currentLvl){
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startRandomWalkPosition);
        //tilemapVisualizer.Clear();
        foreach(var position in floorPositions){
            tilemapVisualizer.PaintFloorTiles(floorPositions, true);
        }
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }


    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position){
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        /*System.Random randomizer = new System.Random();*/

       /* if (this.isSeeded)
        {
            UnityEngine.Random.InitState(this.seed);
        }*/

        for (int i=0; i<randomWalkParameters.iterations; i++){
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, randomWalkParameters.walkLength, this.isSeeded, this.seed);
            floorPositions.UnionWith(path);
            if(randomWalkParameters.startRandomlyEachIteration){
                /*currentPosition = floorPositions.ElementAt(randomizer.Next(0,floorPositions.Count));*/
                currentPosition = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
            }
        }

        return floorPositions;
    }

    protected HashSet<Vector2Int> CreateSeperateRoomTiles(Vector2Int swStartPosition)
    {
        HashSet<Vector2Int> roomTiles = RunRandomWalk(randomWalkParameters, swStartPosition);
        foreach (var position in roomTiles)
        {
            tilemapVisualizer.PaintFloorTiles(roomTiles, true);
        }
        WallGenerator.CreateWalls(roomTiles, tilemapVisualizer);
        return roomTiles;
    }

}
