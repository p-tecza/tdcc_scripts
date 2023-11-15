
using UnityEngine;

public class NPCInteractionController : MonoBehaviour
{
    [SerializeField]
    private GameObject interactionWindowObject;

    public void DisableInteractionWindow()
    {
        this.interactionWindowObject.SetActive(false);
    }

    public void EnableInteractionWindow()
    {
        this.interactionWindowObject.SetActive(true);
    }

}