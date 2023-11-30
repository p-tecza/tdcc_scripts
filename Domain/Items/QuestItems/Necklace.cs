using UnityEngine;

public class Necklace : QuestItem
{

    public void Start()
    {
        if (this.gameObject.name.Contains("(Clone)"))
        {
            this.gameController.AddNewHint(gameObject.transform.position, gameObject);
            base.CreateDropAnimation();
        }

        Debug.Log("NECKLACE: TWORZE SIE");
        
    }
    public void OnDestroy()
    {
        Debug.Log("NECKLACE: NISZCZE SIE");
    }



}