using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndestroyableSceneController : MonoBehaviour
{
    public static bool isThisGameFromSave = false;
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
