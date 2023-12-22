using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;
    public void LoadSave()
    {
        //tu bedzie pobieranie danych, trzeba bedzie to jakos rozsadnie przekazac
        /*this.gameController.GenerateDungeonForSavePurposes();*/
        UndestroyableSceneController.isThisGameFromSave = true;
        /*Debug.Log("IS THIS W MAIN MENU: " + UndestroyableSceneController.isThisGameFromSave);*/
        GameSceneManager.instance.LoadGameScene();


    }

}
