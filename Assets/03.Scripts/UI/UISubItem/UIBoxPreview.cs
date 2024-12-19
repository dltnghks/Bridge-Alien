using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        _uiTimer = GetObject((int)GameObjects.UITimer).GetOrAddComponent<UITimer>();

        return true;
    }

    public void SetPreviewBoxInfo(MiniGameUnloadBox box)
    {
        GetText((int)Texts.BoxNumberText).SetText(box.Info.BoxNumber.ToString());
        GetText((int)Texts.BoxTypeText).SetText(box.Info.BoxType.ToString());
        GetText((int)Texts.RegionText).SetText(box.Info.Region.ToString());
    }
}
