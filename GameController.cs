using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private FullDungeonGenerator dungeonGenerator;

    [SerializeField]
    private CharacterController characterController;

    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    [Range(1f, 10f)]
    private float cameraZoom = 1f;

    // Start is called before the first frame update
    void Start()
    {
        dungeonGenerator.GenerateDungeon();
        Vector3 startPosition = dungeonGenerator.GetStartingPosition();
        playerObject.transform.position = startPosition;
        mainCamera.transform.position = startPosition;
        characterController.SetUpCharacter();
        characterController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.position = 
            new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, -cameraZoom);
    }
}
