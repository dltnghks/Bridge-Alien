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
        
        BindImage(typeof(Images));
        
        return _init;
    }

    public void SetSkillInfo(SkillBase[] skillList)
    {
        if (_init == false) return;
        if (skillList == null || skillList.Length == 0) return;
        
        _skillList = skillList;

        foreach (var skill in _skillList)
        {
            if (skill is EmergencyRocketSkill rocketSkill)
            {
                rocketSkill.OnCooldownChanged += SetRocketSkillButtonDuration;
                GetImage((int)Images.RocketDurationImage).sprite = rocketSkill.SkillData.Icon;
            }
            else if (skill is EmergencyRepairSkill repairSkill)
            {
                repairSkill.OnCooldownChanged += SetRepairSkillButtonDuration;
                GetImage((int)Images.RepairDurationImage).sprite = repairSkill.SkillData.Icon;
            }
        }
    }

    public void SetSkillAction(Action<int> skillAction)
    {
        _skillAction = skillAction;
    }
    
    public void SetRocketSkillButtonDuration(float currentDuration, float maxDuration)
    {
        if (_init)
        {
            GetImage((int)Images.RocketDurationImage).fillAmount = 1 - currentDuration / maxDuration;
        }
    }
    
    public void SetRepairSkillButtonDuration(float currentDuration, float maxDuration)
    {
        if (_init)
        {
            GetImage((int)Images.RepairDurationImage).fillAmount = 1 - currentDuration / maxDuration;
        }
    }

    public void OnRepairSkill()
    {
        Debug.Log("Repair");
        _skillAction?.Invoke((int)Buttons.RepairSkillButton);
    }

    public void OnRocketSkill()
    {
        Debug.Log("Rocket");
        _skillAction?.Invoke((int)Buttons.RocketSkillButton);
    }
}
