

using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    public string npcName;

    public static List<string> npcTypes = new List<string>()
    {
        "Trainer", "Trader", "Wanderer", "Astray"
    };
    // Trainer -> possibility to upgrade some of your skills
    // Trader -> possibility to buy an item with discounted price
    // Wanderer -> basic talk, nothing game-changing
    // Astray -> grants an item after returning something he lost
    public abstract void Interact();

    public static int DetermineRandomNPC()
    {
        return UnityEngine.Random.Range(0, npcTypes.Count);
    }

}