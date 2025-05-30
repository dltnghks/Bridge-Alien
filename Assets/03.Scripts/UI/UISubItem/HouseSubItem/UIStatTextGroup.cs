using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStatTextGroup : UISubItem
{
    enum Texts
    {
        ExperienceValueText,
        GravityValueText,
        IntelligenceValueText,
        LuckValueText,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        
        return true;
    }

    public void SetTexts(PlayerTaskData taskData)
    {
        Init();
        
    }
    
}
