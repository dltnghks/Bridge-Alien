using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoxPreview : UISubItem
{
    enum GameObjects
    {
        UITimer,
    }

    enum Texts
    {
        BoxNumberText,
        RegionText,
        BoxTypeText,
    }

    enum Images
    {
        BoxImage
    }
    
    private UITimer _uiTimer;
    public UITimer UITimer
    {
        get
        {
            Init();
            return _uiTimer;
        }
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));
        
        _uiTimer = GetObject((int)GameObjects.UITimer).GetOrAddComponent<UITimer>();

        return true;
    }

    public void SetPreviewBoxInfo(MiniGameUnloadBox box)
    {
        Image boxImage = GetImage((int)Images.BoxImage);
        Sprite sprite = box.SpriteRenderer.sprite;
        if (sprite != null)
        {
            boxImage.sprite = box.SpriteRenderer.sprite;

            // UI Image 크기 조정
            RectTransform rectTransform = boxImage.GetComponent<RectTransform>();
            rectTransform.sizeDelta =
                new Vector2(sprite.rect.width/1.5f, sprite.rect.height/1.5f);
        }

        GetText((int)Texts.BoxNumberText).SetText(box.Info.BoxNumber);
        GetText((int)Texts.BoxTypeText).SetText(box.Info.GetBoxType());
        GetText((int)Texts.RegionText).SetText(box.Info.GetBoxRegion());
    }
}
