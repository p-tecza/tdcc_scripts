using TMPro;
using UnityEngine;

public class TextHighlight : MonoBehaviour
{

    public void HighlightTextOnHover()
    {
        TMP_Text text = gameObject.GetComponent<TMP_Text>();
        text.color = Color.white;
    }

    public void UnhighlightTextOutOfHover()
    {
        TMP_Text text = gameObject.GetComponent<TMP_Text>();
        float r = (float)(125f / 255f);
        float g = (float)(125f / 255f);
        float b = (float)(125f / 255f);
        text.color = new UnityEngine.Color(r,g,b,1);
    }

}