using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintsDataProvider : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    private HintsController hintsController;

    [SerializeField]
    private GameObject childCanvasOfAssociatedGameObject;

    public void ProvideHintData()
    {
        string itemName = gameObject.GetComponent<TMP_Text>().text;
        AllItemsData itemsData = this.gameController.GetAllItemsData();
        Sprite sprite = gameObject.transform.parent.parent.parent.GetComponent<SpriteRenderer>().sprite;

        int questItemsLayer = LayerMask.NameToLayer("QuestItems");
        int objectLayer = childCanvasOfAssociatedGameObject.transform.parent.gameObject.layer;

        Debug.Log("QI LAYER: " + questItemsLayer);
        Debug.Log("GO LAYER: " + objectLayer);

        if (itemName.Equals("Hp Potion") || itemName.Equals("Star"))
        {
            CollectableData collectableData = FindCollectableData(itemName, itemsData.collectables);
            this.hintsController.SetCollectableDescriptionContent(collectableData, sprite);
        }
        else if (objectLayer == questItemsLayer)
        {
            QuestItemData questItemData = this.gameController.itemRepository.GetQuestItemByName(itemName);
            Debug.Log("QID: " + questItemData.description);
            this.hintsController.SetQuestItemDescriptionContent(questItemData, sprite);
        }
        else
        {
            ItemData collectableData = FindItemData(itemName, itemsData.items);
            this.hintsController.SetItemDescriptionContent(collectableData, sprite);
        }
    }

    private CollectableData FindCollectableData(string itemName, List<CollectableData> collectables)
    {
        if (collectables == null)
        {
            Debug.Log("NULL ITEMS");
            return new CollectableData();
        }

        foreach (CollectableData collectable in collectables)
        {
            if(collectable.name == itemName)
            {
                return collectable;
            }
        }
        return new CollectableData();
    }

    private ItemData FindItemData(string itemName, List<ItemData> items)
    {
        if (items == null)
        {
            Debug.Log("NULL ITEMS");
            return new ItemData();
        }

        foreach (ItemData item in items)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }
        return new ItemData();
    }

}