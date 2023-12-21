using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    public void GameOverScreenPopUp()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
    }
}
