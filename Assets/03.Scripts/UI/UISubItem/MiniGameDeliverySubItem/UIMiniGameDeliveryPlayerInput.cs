using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMiniGameDeliveryPlayerInput : UIPlayerInput
{
    private Action<int> _skillAction;
    private SkillBase[] _skillList;

    enum Images
    {
        RepairDurationImage,
        RocketDurationImage
    }

    enum Buttons
    {
        RepairSkillButton,
        RocketSkillButton,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        _init = true;
        
        return _init;
    }

    public void SetSkillInfo(SkillBase[] skillList)
    {
        if (_init == false) return;
        if (skillList == null || skillList.Length == 0) return;
        
        _skillList = skillList;

        foreach (var skill in _skillList)
        {
            
        }
    }
}
