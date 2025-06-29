using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBlurBackground : UIPopup
{
    public bool IsInputEnabled { get; set; }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        IsInputEnabled = true;
        
        gameObject.BindEvent(OnClickBackground);
        
        return true;
    }

    private void OnClickBackground()
    {
        if (IsInputEnabled)
        {
            Managers.UI.ClosePopupUI();
        }
    } 
}
