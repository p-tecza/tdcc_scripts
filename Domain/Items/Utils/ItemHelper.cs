

public class ItemHelper
{
    public static string GetNameOfItem(string nameOfObject)
    {
        try
        {
            string name = nameOfObject.Split("(Clone)")[0];
            return name == "HealthPotion" ? "Hp Potion" : name;
        }
        catch
        {
            return nameOfObject;
        }
    }
}