using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkModuleSkillButton : UISubItem
{
    enum Images
    {
        SkillIconImage,
    }
    
    enum Texts
    {
        SkillLevelText,
        SkillNameText,
        SkillDescriptionText,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        BindImage(typeof(Images));

        return true;
    }

    public void SetWorkModuleSkillInfo(SkillData skillData)
    {
        int skillLevel = Managers.Player.PlayerData.MiniGameUnloadSkillLevel[skillData.Type];
        GetImage((int)Images.SkillIconImage).sprite = skillData.Icon;
        
        GetText((int)Texts.SkillNameText).SetText(skillData.Name);
        GetText((int)Texts.SkillLevelText).SetText(skillLevel.ToString());

        if (skillData is DurationSkillData durationSkillData)
        {
            GetText((int)Texts.SkillDescriptionText).SetText(string.Format(durationSkillData.Description, durationSkillData.MaxDuration));
        }
        else if (skillData is ChargeSkillData chargeSkillData)
        {
            GetText((int)Texts.SkillDescriptionText).SetText(string.Format(chargeSkillData.Description, chargeSkillData.MaxCharges));
        }
    }
}
