using UnityEngine;

public class QuestItem : MonoBehaviour
{
    [SerializeField]
    protected GameController gameController;

    public string questItemName;
    private Vector2 definedPopUpForce = new Vector2(0, 1);
    private float forceMultiplier = 3f;
    private float returnForceDelay = 0.15f;

    protected void CreateDropAnimation()
    {
        Debug.Log("DODAJE SILE");
        gameObject.GetComponent<Rigidbody2D>().AddForce(this.definedPopUpForce * this.forceMultiplier, ForceMode2D.Impulse);
        Invoke("OppositeForce", returnForceDelay);
    }

    private void OppositeForce()
    {
        Debug.Log("ODEJMUJE SILE");
        gameObject.GetComponent<Rigidbody2D>().AddForce(-2 * this.definedPopUpForce * this.forceMultiplier, ForceMode2D.Impulse);
        Invoke("ResetForce", returnForceDelay);
    }
    private void ResetForce()
    {
        Debug.Log("ZATRZYMUJE");
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

}