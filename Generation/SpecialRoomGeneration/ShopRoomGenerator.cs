using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopRoomGenerator : MonoBehaviour
{
    [SerializeField]
    protected GameObject trainerNPC;
    [SerializeField]
    protected GameObject traderNPC;
    [SerializeField]
    protected GameObject wandererNPC;
    [SerializeField]
    protected GameObject astrayNPC;
    [SerializeField]
    private GameObject priceText;
    [SerializeField]
    private Canvas priceCanvas;
    [SerializeField]
    private Treasure treasure;
    [SerializeField]
    private GameController gameController;

    public GameObject npcInShopRoom;
    public List<GameObject> itemsInShopRoom;


    private TilemapVisualizer tilemapVisualizer;

    public Room GenerateShop(Room room, TilemapVisualizer tilemapVisualizer, GameObject parentObjectForInstantiated)
    {
        this.itemsInShopRoom = new List<GameObject>();
        this.tilemapVisualizer = tilemapVisualizer;

        List<GameObject> treasureList = treasure.possibleTreasures;
        

        ShopRoom shopRoom = new ShopRoom(room);
        Vector2Int roomCenter = RoomHelper.DetermineRoomCenter(room.FloorTiles);

        FindProperSpaceForShop3x3(roomCenter, room);
        Vector3 npcSpot = new Vector3(roomCenter.x + 0.5f, roomCenter.y + 0.5f, 0);
        GameObject npcObject = PickProperObject(shopRoom.npcType);
        if(npcObject != null) 
        {
            AllInteractiveDialogData allNPCDialogData = DataReader.ReadAllNPCDialogData();
            GameObject refNPCObject = Instantiate(npcObject, npcSpot, Quaternion.identity);
            
            refNPCObject.transform.SetParent(parentObjectForInstantiated.transform, true);
            //
            refNPCObject.GetComponent<NPC>().SetNPCData(allNPCDialogData);
            Vector3 NPCposition = refNPCObject.transform.GetChild(0).transform.position;
            refNPCObject.transform.position += (refNPCObject.transform.position-NPCposition);
            this.npcInShopRoom = refNPCObject;
        }

        Vector3 itemSpot = new Vector3(roomCenter.x + 0.5f, roomCenter.y + -1.5f, 0);
        Vector3 starSpot = new Vector3(roomCenter.x - 0.5f, roomCenter.y + -0.5f, 0);
        Vector3 hpPotionSpot = new Vector3(roomCenter.x + 1.5f, roomCenter.y + -0.5f, 0);

        GameObject itemObject = treasureList[shopRoom.itemId];
        GameObject starObject = treasure.star;
        GameObject hpPotionObject = treasure.healthPotion;


        if (itemObject != null) InstantiateShopItemWithPrice(itemObject, itemSpot,
            ShopRoom.costs["item"], parentObjectForInstantiated);
        if (starObject != null) InstantiateShopItemWithPrice(starObject, starSpot,
            ShopRoom.costs["star"], parentObjectForInstantiated);
        if (hpPotionObject != null) InstantiateShopItemWithPrice(hpPotionObject, hpPotionSpot,
            ShopRoom.costs["hpPotion"], parentObjectForInstantiated);



        /*priceCanvasObject.transform.SetParent(parentObjectForInstantiated.transform, true);*/
        return room;
    }

    private void InstantiateShopItemWithPrice(GameObject item, Vector3 position, int price,
        GameObject parentObjectForInstantiated)
    {

        GameObject instantiatedObject = Instantiate(item, position, Quaternion.identity);
        instantiatedObject.transform.SetParent(parentObjectForInstantiated.transform, true);
        instantiatedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        instantiatedObject.tag = "ShopItem";
        InstantiatePriceOfItem(instantiatedObject, position, price);
        this.gameController.AddNewHint(position, instantiatedObject);
        this.itemsInShopRoom.Add(instantiatedObject);
    }

    private void InstantiatePriceOfItem(GameObject itemInstantiated, Vector3 position, int price)
    {

        GameObject priceCanvasObject = Instantiate(this.priceCanvas.gameObject,
            this.priceCanvas.transform.position, Quaternion.identity);

        BoxCollider2D itemCollider = itemInstantiated.GetComponent<BoxCollider2D>();
        float offset = itemCollider.bounds.size.y / 2 + 0.3f;
        Vector3 textPosition = new Vector3(position.x, position.y - offset, position.z);
        GameObject textObject = Instantiate(this.priceText, textPosition, Quaternion.identity);
        textObject.GetComponent<TMP_Text>().text = price.ToString() + " G";
        textObject.transform.SetParent(priceCanvasObject.transform, true);
        textObject.name = "PriceText" + itemInstantiated.name;
        priceCanvasObject.transform.SetParent(itemInstantiated.transform, true);
    }

    private GameObject PickProperObject(string npcType)
    {
        switch(npcType)
        {
            case "Trainer":
                return trainerNPC;
            case "Trader":
                return traderNPC;
            case "Wanderer":
                return wandererNPC;
            case "Astray":
                return astrayNPC;
            default: return null;
        }
    }

    private Room FindProperSpaceForShop3x3(Vector2Int roomCenter, Room room)
    {
        bool appendNeeded = false;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int consideredTile = new Vector2Int(roomCenter.x + i, roomCenter.y + j);
                if (!room.WallTiles.Contains(consideredTile))
                {
                    room = AppendRoomTilesWithSingleTile(consideredTile, room);
                    appendNeeded = true;
                }
            }
        }

        if (appendNeeded)
        {
            FixWalls(room);
        }

        return room;
    }

    private Room AppendRoomTilesWithSingleTile(Vector2Int positionToAppend, Room room)
    {
        room.WallTiles.Add(positionToAppend);
        this.tilemapVisualizer.PaintFloorTiles(new List<Vector2Int>() { positionToAppend }, room.isComplex);
        return room;
    }

    private Room FixWalls(Room room)
    {
        HashSet<Vector2Int> fieldsToRepair = FindTilesToRepair(room);
        room.WallTiles.AddRange(fieldsToRepair);
        WallGenerator.CreateWalls(room.FloorTiles, this.tilemapVisualizer, room.teleports);
        return room;
    }

    private HashSet<Vector2Int> FindTilesToRepair(Room room)
    {
        HashSet<Vector2Int> tilesToRepair = new HashSet<Vector2Int>();
        foreach(Vector2Int tile in room.FloorTiles)
        {
            Vector2Int relativeLeft = new Vector2Int(tile.x - 1, tile.y);
            Vector2Int relativeRight = new Vector2Int(tile.x + 1, tile.y);
            Vector2Int relativeTop = new Vector2Int(tile.x, tile.y + 1);
            Vector2Int relativeBottom = new Vector2Int(tile.x, tile.y - 1);
            if (!room.WallTiles.Contains(relativeTop) && !room.FloorTiles.Contains(relativeTop))
            {
                tilesToRepair.Add(relativeTop);
            }
            if (!room.WallTiles.Contains(relativeRight) && !room.FloorTiles.Contains(relativeRight))
            {
                tilesToRepair.Add(relativeRight);
            }
            if (!room.WallTiles.Contains(relativeLeft) && !room.FloorTiles.Contains(relativeLeft))
            {
                tilesToRepair.Add(relativeLeft);
            }
            if (!room.WallTiles.Contains(relativeBottom) && !room.FloorTiles.Contains(relativeBottom))
            {
                tilesToRepair.Add(relativeBottom);
            }
        }
        return tilesToRepair;
    }

    public void RemoveShopItemFromState(GameObject pickedUpShopObject)
    {
        if(this.itemsInShopRoom.Contains(pickedUpShopObject))
        {
            this.itemsInShopRoom.Remove(pickedUpShopObject);
        }
    }
}