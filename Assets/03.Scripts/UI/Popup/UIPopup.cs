using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPopup : UIBase
{
    // 팝업 창 뒤에 있는 투명한 배경에 상호작용이 가능 여부
    [SerializeField] private bool IsBlurBGInputEnable = false;
    
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetInputBackground(IsBlurBGInputEnable);

        return true;
    }

    // 팝업에서만 사용하는 초기화 함수
    public virtual void Init(object data)
    {
        Init();
    }

    
    public virtual void ClosePopupUI()
    {
        Logger.Log("ClosePopupUI");
        Managers.UI.ClosePopupUI(this);
    }
    
}
