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
        UISmallBoxPreview1,
        UISmallBoxPreview2,
        UISmallBoxPreview3,
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

    private List<UISmallBoxPreview> _uiSmallBoxPreviewList = new List<UISmallBoxPreview>();
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(GameObjects));
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        _uiSmallBoxPreviewList.Add(GetObject((int)GameObjects.UISmallBoxPreview1).GetOrAddComponent<UISmallBoxPreview>());
        _uiSmallBoxPreviewList.Add(GetObject((int)GameObjects.UISmallBoxPreview2).GetOrAddComponent<UISmallBoxPreview>());
        _uiSmallBoxPreviewList.Add(GetObject((int)GameObjects.UISmallBoxPreview3).GetOrAddComponent<UISmallBoxPreview>());

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

        int index = boxList.Count-1;
        for (int i = 0; i < _uiSmallBoxPreviewList.Count; i++)
        {
            MiniGameUnloadBox box = null;
            if (0 <= index && index < boxList.Count)
            {
                box = boxList[index--];
            }
            _uiSmallBoxPreviewList[i].SetBoxPreview(box);   
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
            }
        }
        
        GetImage((int)Images.BoxImage).color = new Color(0, 0, 0, 0);
        boxImage.sprite = sprite;
        GetText((int)Texts.BoxNumberText).SetText(boxNumber);
        GetText((int)Texts.RegionText).SetText(boxRegion);

        if (sprite != null)
        {
            GetImage((int)Images.BoxImage).color = new Color(1, 1, 1, 1);
        }
    }
}
