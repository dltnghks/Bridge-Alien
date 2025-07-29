using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkModuleSkillButton : UIActiveButton
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

    public Define.MiniGameSkillType SkillType { get; private set; }
    private UIWorkModulePopup _workModuleController;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindImage(typeof(Images));

        gameObject.BindEvent(OnSelectTab);

        Deselect();

        return true;
    }

    public void Init(UIWorkModulePopup workModuleController)
    {
        Init();

        _workModuleController = workModuleController;
    }

    public void SetWorkModuleSkillInfo(SkillData skillData)
    {
        int skillLevel = Managers.Player.PlayerData.MiniGameUnloadSkillLevel[skillData.Type];
        GetImage((int)Images.SkillIconImage).sprite = skillData.Icon;

        GetText((int)Texts.SkillNameText).SetText(skillData.Name);
        GetText((int)Texts.SkillLevelText).SetText($"Level : {skillLevel}");

        if (skillData is DurationSkillData durationSkillData)
        {
            GetText((int)Texts.SkillDescriptionText).SetText(string.Format(durationSkillData.Description, durationSkillData.GetSkillValue(skillLevel)));
        }
        else if (skillData is ChargeSkillData chargeSkillData)
        {
            GetText((int)Texts.SkillDescriptionText).SetText(string.Format(chargeSkillData.Description, chargeSkillData.GetSkillValue(skillLevel)));
        }

        SkillType = skillData.Type;
    }

    private void OnSelectTab()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _workModuleController.SelectSkillButton(this);
    }

    public void Select()
    {
        Activate();
    }

    public void Deselect()
    {
        Deactivate();
    }
}
