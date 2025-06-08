using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISmallBoxPreview : UISubItem
{
    enum Images
    {
        BoxImage,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(Images));
        SetBoxPreview(null);

        return true;
    }
    public void SetBoxPreview(MiniGameUnloadBox box)
    {
        Sprite sprite = null;
        if (box != null)
        {
            SpriteRenderer spriteRenderer = box.GetComponent<SpriteRenderer>();
            sprite = spriteRenderer.sprite;
        }

        GetImage((int)Images.BoxImage).color = new Color(0, 0, 0, 0);
        GetImage((int)Images.BoxImage).sprite = sprite;
        if (sprite != null)
        {
            GetImage((int)Images.BoxImage).color = new Color(1, 1, 1, 1);
        }
    }

}
