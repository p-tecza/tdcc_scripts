using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseWindow;
    public static PauseManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Pause()
    {
        pauseWindow.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Unpause()
    {
        pauseWindow.SetActive(false);
        Time.timeScale = 1f;
    }

    public void SaveAndReturnToMainMenu()
    {
        Debug.Log("TODO ZAPISYWANIE!!!");
        GameSceneManager.instance.LoadMainMenuScene();
    }

}
