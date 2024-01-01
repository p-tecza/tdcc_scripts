using UnityEngine;

public class GameObjectGenerator : MonoBehaviour
{
    public GameObject necklaceObject;

    public GameObject GenerateEntity(string entityName)
    {
        switch(entityName)
        {
            case "Necklace":
                return GenerateNecklaceObject();
            default: return null;
                
        }
    }
    private GameObject GenerateNecklaceObject()
    {
        return Instantiate(necklaceObject);
    }

}