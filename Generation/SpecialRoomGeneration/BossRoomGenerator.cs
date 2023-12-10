using System.Collections.Generic;
using UnityEngine;

public class BossRoomGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject nextLevelTeleport;

    public void SetUpBossRoom(Room room, GameObject parentObjectForInstantiated)
    {
        this.GenerateTeleportToNextLevel(room, parentObjectForInstantiated);
    }

    public void GenerateTeleportToNextLevel(Room room, GameObject parentObjectForInstantiated)
    {
        
        HashSet<Vector2Int> roomTiles = room.FloorTiles;
        Vector2Int pseudoCenter = RoomHelper.DetermineRoomCenter(roomTiles);
        Vector3 instantiatedPosition = new Vector3(pseudoCenter.x, pseudoCenter.y, 0);
        GameObject newGameObject = Instantiate(this.nextLevelTeleport);
        newGameObject.transform.position = instantiatedPosition;
        newGameObject.transform.SetParent(parentObjectForInstantiated.transform, true);

    }

}