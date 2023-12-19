using System.Collections.Generic;
using UnityEngine;

public class BossRoomGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject nextLevelTeleport;

    [SerializeField]
    private GameObject firstBossPrefab;

    private GameObject thisRoomNextLevelTeleport;

    [SerializeField]
    private PlayerController playerController;

    public void SetUpBossRoom(Room room, GameObject parentObjectForInstantiated)
    {
        this.GenerateBoss(room, parentObjectForInstantiated);
        this.GenerateTeleportToNextLevel(room, parentObjectForInstantiated);
    }

    private void GenerateBoss(Room room, GameObject parentObjectForInstantiated)
    {
        Vector2Int pseudoCenter = RoomHelper.DetermineRoomCenter(room.FloorTiles);
        Vector3 instantiatedPosition = new Vector3(pseudoCenter.x, pseudoCenter.y, 0);
        GameObject bossObject = Instantiate(firstBossPrefab);
        bossObject.transform.position = instantiatedPosition;
        bossObject.transform.SetParent(parentObjectForInstantiated.transform, true);
        room.enemies.Add(bossObject);
    }

    public void GenerateTeleportToNextLevel(Room room, GameObject parentObjectForInstantiated)
    {
        HashSet<Vector2Int> roomTiles = room.FloorTiles;
        Vector2Int pseudoCenter = RoomHelper.DetermineRoomCenter(roomTiles);
        Vector3 instantiatedPosition = new Vector3(pseudoCenter.x, pseudoCenter.y, 0);
        GameObject newGameObject = Instantiate(this.nextLevelTeleport);
        newGameObject.transform.position = instantiatedPosition;
        newGameObject.transform.SetParent(parentObjectForInstantiated.transform, true);
        this.thisRoomNextLevelTeleport = newGameObject;
    }
    public void OpenNextLevelTeleport()
    {
        this.thisRoomNextLevelTeleport.SetActive(true);
        Invoke("EnableTp", 1f);
    }

    private void EnableTp()
    {
        this.playerController.EnableNextLevelTeleport();
    }
}