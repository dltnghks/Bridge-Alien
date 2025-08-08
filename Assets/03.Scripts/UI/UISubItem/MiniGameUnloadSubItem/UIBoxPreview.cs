using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        SetPreviewBoxInfo(null);

        return true;
    }

    public void UpdateUI(List<MiniGameUnloadBox> boxList)
    {
        if (boxList.Count > 0) {
            SetPreviewBoxInfo(boxList[boxList.Count-1]);
        } else {
            SetPreviewBoxInfo(null);
        }
    }

    public void SetPreviewBoxInfo(MiniGameUnloadBox box)
    {
        Image boxImage = GetImage((int)Images.BoxImage);
        Sprite sprite = null;
        string boxNumber = "";
        string boxRegion = "";
        if (box != null)
        {
            sprite = box.GetComponent<SpriteRenderer>().sprite;
            if (sprite != null)
            {
                boxImage.sprite = sprite;
                boxNumber = box.Info.BoxNumber;
                boxRegion = box.Info.GetBoxRegion();
                GetImage((int)Images.BoxImage).color = new Color(0, 0, 0, 0);

                boxImage.sprite = sprite;
                GetText((int)Texts.BoxNumberText).SetText(boxNumber);
                GetText((int)Texts.RegionText).SetText($"{boxRegion} 레일");
            }
        }
        else
        {
            GetImage((int)Images.BoxImage).color = new Color(0, 0, 0, 0);
            GetText((int)Texts.BoxNumberText).SetText("");
            GetText((int)Texts.RegionText).SetText("");
        }

        if (sprite != null)
        {
            GetImage((int)Images.BoxImage).color = new Color(1, 1, 1, 1);
        }
    }
}
