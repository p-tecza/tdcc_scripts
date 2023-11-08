using UnityEngine;
using UnityEngine.UI;

public class HintsController : MonoBehaviour
{

    public Canvas descriptionCanvas;
    /*public Button hintButton;*/

/*    void Start()
    {
        hintButton.onClick.AddListener(ToggleItemDescription);
    }*/
  /*  void Update()
    {
        DetectHintClick();
    }

    public void DetectHintClick()
    {

    }
*/
    public void ToggleItemDescription()
    {
        descriptionCanvas.enabled = true;
    }
}