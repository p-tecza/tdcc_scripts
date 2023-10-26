

using JetBrains.Annotations;
using System.Collections.Generic;

public class SpecialRoomDeterminer
{
    public List<int> treasureRoomIDs = new List<int>();
    public int bossRoomID = 0;
    public int shopRoomID = 0;
    public int startingRoomID = 0;

    private List<int> alreadyUsedRooms = new List<int>();

    public int DetermineTreasureRoom(int startingRoomID, int bossRoomID, List<int> currentTreasureRooms, int roomsAmount)
    {
        int treasureRoomId;
        int infiniteLoopFlag = 0;
        bool notFoundRandomly = false;

        if (currentTreasureRooms.Count >= roomsAmount - 2) return currentTreasureRooms[0];
        do
        {
            treasureRoomId = UnityEngine.Random.Range(1, roomsAmount - 1);
            infiniteLoopFlag++;
            if (infiniteLoopFlag >= roomsAmount * 10)
            {
                notFoundRandomly = true;
            }
        } while (treasureRoomId == startingRoomID || treasureRoomId == bossRoomID || currentTreasureRooms.Contains(treasureRoomId));

        if (notFoundRandomly)
        {
            for (int i = 0; i < roomsAmount; i++)
            {
                if (alreadyUsedRooms.Contains(i)) continue;
                treasureRoomId = i;
                break;
            }
        }

        alreadyUsedRooms.Add(treasureRoomId);

        return treasureRoomId;
    }

    public int DetermineStartingRoom()
    {
        alreadyUsedRooms.Add(0);
        return 0;
    }

    public int DetermineBossRoom()
    {
        int bossRoomID = GridAlgorithm.gg.FindFurthestNode();
        alreadyUsedRooms.Add(bossRoomID);
        return bossRoomID; 
    }

    public int DetermineShopRoom()
    {
        int possibleNodes = GridAlgorithm.gg.distancesToNodes.Count;
        int infiniteLoopFlag = 0;
        bool notFoundRandomly = false;
        int shopRoomID;
        do
        {
            shopRoomID = UnityEngine.Random.Range(0, possibleNodes);
            infiniteLoopFlag++;
            if (infiniteLoopFlag >= possibleNodes * 10)
            {
                notFoundRandomly = true;
            }
        }
        while (alreadyUsedRooms.Contains(shopRoomID));

        if (notFoundRandomly)
        {
            for(int i = 0; i < possibleNodes; i++)
            {
                if (alreadyUsedRooms.Contains(i)) continue;
                shopRoomID = i;
                break;
            }
        }

        alreadyUsedRooms.Add(shopRoomID);
        return shopRoomID;

    }

}