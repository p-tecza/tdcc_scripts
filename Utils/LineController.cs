using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lr;
    public int counterLine = 0;

    public void DrawLine(Vector3 first, Vector3 second)
    {
        lr.sharedMaterial.SetColor("_Color", new Color(0f, 0f, 0f, 0f));
        lr.SetPosition(this.counterLine++, first);
        lr.SetPosition(this.counterLine++, second); 
    }

    public void ResetLines(int newSize)
    {
        lr.positionCount = 0;
        lr.positionCount = newSize;
    }
}
