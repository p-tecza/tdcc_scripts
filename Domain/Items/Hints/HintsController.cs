using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HintsController : MonoBehaviour
{

    public GameObject descriptionCanvas;
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

    public void SetCollectableDescriptionContent(CollectableData collectableData, Sprite sprite)
    {
        Debug.Log("DOSTANIETE: " + collectableData.name);

        Transform descContentTransform = this.descriptionCanvas.transform.Find("DescriptionContent");
        TMP_Text descItemName = descContentTransform.Find("DescriptionItemName").gameObject.GetComponent<TMP_Text>();
        TMP_Text descText = descContentTransform.Find("DescriptionText").gameObject.GetComponent<TMP_Text>();
        Image spriteRenderer = descContentTransform.Find("DescriptionItemIcon").gameObject.GetComponent<Image>();
        spriteRenderer.sprite = sprite;

        descItemName.text = collectableData.name;
        descText.text = collectableData.description;
    }

    public void SetItemDescriptionContent(ItemData itemData, Sprite sprite)
    {
        Debug.Log("DOSTANIETE: " + itemData.name);

        Transform descContentTransform = this.descriptionCanvas.transform.Find("DescriptionContent");
        TMP_Text descItemName = descContentTransform.Find("DescriptionItemName").gameObject.GetComponent<TMP_Text>();
        TMP_Text descText = descContentTransform.Find("DescriptionText").gameObject.GetComponent<TMP_Text>();
        Image spriteRenderer = descContentTransform.Find("DescriptionItemIcon").gameObject.GetComponent<Image>();
        spriteRenderer.sprite = sprite;

        descItemName.text = itemData.name;
        descText.text = itemData.description;

    }

    public void SetQuestItemDescriptionContent(QuestItemData itemData, Sprite sprite)
    {
        Transform descContentTransform = this.descriptionCanvas.transform.Find("DescriptionContent");
        TMP_Text descItemName = descContentTransform.Find("DescriptionItemName").gameObject.GetComponent<TMP_Text>();
        TMP_Text descText = descContentTransform.Find("DescriptionText").gameObject.GetComponent<TMP_Text>();
        Image spriteRenderer = descContentTransform.Find("DescriptionItemIcon").gameObject.GetComponent<Image>();
        spriteRenderer.sprite = sprite;

        descItemName.text = itemData.name;
        descText.text = itemData.description;
    }

    public void ToggleItemDescription()
    {
        descriptionCanvas.SetActive(true);
    }

    public void DisableItemDescription()
    {
        descriptionCanvas.SetActive(false);
    }
}