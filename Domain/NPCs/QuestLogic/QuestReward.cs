public class QuestReward
{
    private string rewardType;
    private int money;
    private ItemData item;
    public QuestReward(string rewardType, int money, string itemName)
    {
        if(rewardType == "Money")
        {
            this.item = null;
            this.money = money;
            this.rewardType = rewardType;
        }
        else if(rewardType == "Item")
        {
            ItemRepository itemRepository = new ItemRepository();
            this.item = itemRepository.GetItemByName(itemName);
            this.money = 0;
            this.rewardType = rewardType;
        }
    }

    public string GetRewardType()
    {
        return this.rewardType;
    }

    public int GetMoneyAmount()
    {
        return this.money;
    }

    public ItemData GetItem()
    {
        return this.item;
    }

}