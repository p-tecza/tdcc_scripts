using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile, wallTop, passageTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions){
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile){
        foreach(var position in positions){
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position){
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }


    public void PaintSingleTileWithColor(Vector2Int position, Color color)
    {
        var tilePosition = floorTilemap.WorldToCell((Vector3Int)position);
        floorTilemap.SetTileFlags(tilePosition, TileFlags.None);
        floorTilemap.SetColor(tilePosition, color);
    }
    public void PaintSingleWallTileWithColor(Vector2Int position, Color color)
    {
        var tilePosition = wallTilemap.WorldToCell((Vector3Int)position);
        wallTilemap.SetTile(tilePosition, passageTile);
        wallTilemap.SetTileFlags(tilePosition, TileFlags.None);
        wallTilemap.SetColor(tilePosition, color);
    }

    public void TestujeSe(Vector2Int position, Color color)
    {
        var tilePosition = wallTilemap.WorldToCell((Vector3Int)position);
        wallTilemap.SetTileFlags(tilePosition, TileFlags.None);
        wallTilemap.SetColor(tilePosition, color);
    }

    internal void PaintSingleBasicWall(Vector2Int position){
        PaintSingleTile(wallTilemap, wallTop, position);
    }

    public void Clear(){
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }



}
