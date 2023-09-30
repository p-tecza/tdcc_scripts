using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile0, floorTile1, floorTile2, wallTop, wallSide0, wallSide1, passageTile;
    [SerializeField]
    private TileBase complexFloorTile0, complexFloorTile1, complexFloorTile2;
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions, bool isComplex){
        if (isComplex)
        {
            PaintTiles(floorPositions, floorTilemap, new List<TileBase> { complexFloorTile0, complexFloorTile1, complexFloorTile2 });
        }
        else
        {
            PaintTiles(floorPositions, floorTilemap, new List<TileBase> { floorTile0, floorTile1, floorTile2 });
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, List<TileBase> tiles){



        foreach(var position in positions){

            var rand = Random.Range(0, 6);

            if(rand > 4)
            {
                PaintSingleTile(tilemap, tiles[2], position);
            }
            else if(rand > 2)
            {
                PaintSingleTile(tilemap, tiles[1], position);
            }
            else
            {
                PaintSingleTile(tilemap, tiles[0], position);
            }

            
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
    public void DeleteSingleTile(Vector2Int position)
    {
        var tilePosition = wallTilemap.WorldToCell((Vector3Int)position);
        wallTilemap.SetTile(tilePosition, null);
    }

    public void TestujeSe(Vector2Int position, Color color)
    {
        var tilePosition = wallTilemap.WorldToCell((Vector3Int)position);
        wallTilemap.SetTileFlags(tilePosition, TileFlags.None);
        wallTilemap.SetColor(tilePosition, color);
    }

    internal void PaintSingleSideWall(Vector2Int position){
        var rand = Random.Range(0, 6);

        if(rand > 1) 
        {
            PaintSingleTile(wallTilemap, wallSide0, position);
        }
        else
        {
            PaintSingleTile(wallTilemap, wallSide1, position);
        }

        
    }

    internal void PaintSingleTopWall(Vector2Int position)
    {
        PaintSingleTile(wallTilemap, wallTop, position);
    }

    public void Clear(){
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }



}
