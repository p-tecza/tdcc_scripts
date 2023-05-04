using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    protected Vector2Int startRandomWalkPosition = Vector2Int.zero;
    [SerializeField]
    protected LineController lineController;
    

    public void GenerateDungeon(){
        tilemapVisualizer.Clear();
        lineController.ResetLines(0);
        DestroyAllCreatedPrefabs();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

    protected abstract void DestroyAllCreatedPrefabs();

}
