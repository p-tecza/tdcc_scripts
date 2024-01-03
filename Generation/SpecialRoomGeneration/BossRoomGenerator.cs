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
    [SerializeField]
    private GameController gameController;

    private GameObject bossObject;

    public void SetUpBossRoom(Room room, GameObject parentObjectForInstantiated)
    {
        this.GenerateBoss(room, parentObjectForInstantiated);
        this.GenerateTeleportToNextLevel(room, parentObjectForInstantiated);
        /*EnemiesTracker enemiesTracker = this.gameController.GetEnemiesTracker();
        if (!enemiesTracker.CheckIfBossIsDead())
        {
            
        }
        else
        {
            OpenNextLevelTeleport();
        }*/
    }

    private void GenerateBoss(Room room, GameObject parentObjectForInstantiated)
    {
        Vector2Int pseudoCenter = RoomHelper.DetermineRoomCenter(room.FloorTiles);
        Vector3 instantiatedPosition = new Vector3(pseudoCenter.x, pseudoCenter.y, 0);
        GameObject bossObject = Instantiate(firstBossPrefab);
        bossObject.transform.position = instantiatedPosition;
        bossObject.transform.SetParent(parentObjectForInstantiated.transform, true);
        this.bossObject = bossObject;
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

    public void RepairBossRoomStateFromSave(bool isBossDead)
    {
        if (isBossDead && this.bossObject != null)
        {
            this.bossObject.GetComponent<FirstBossEnemy>().AcknowledgeEnemyDeath();
            Destroy(this.bossObject);
            this.bossObject = null;
            OpenNextLevelTeleport();
            EnableTp();
        }
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