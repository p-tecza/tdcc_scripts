using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TeleportMonoBehaviour : MonoBehaviour
{
    [SerializeField]
    private Sprite openImage;
    [SerializeField]
    private Sprite closedImage;

    public Teleport teleportInfo { get; set; }

    public void SetOpenImage()
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = openImage;
    }

    public void SetClosedImage()
    {
        SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
        sr.sprite = closedImage;
    }

}