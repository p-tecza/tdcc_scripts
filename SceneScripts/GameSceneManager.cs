using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    public static GameSceneManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadMainMenuScene()
    {
        this.gameController.ResetAfterMainMenuReturn();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
