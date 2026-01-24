using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkModuleSkillButton : UIActiveButton
{
    private const string HighlightColorHex = "69A96C";

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
    public bool IsLocked { get; private set; }
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
        Init();
        int skillLevel = Managers.Player.PlayerData.MiniGameUnloadSkillLevel[skillData.Type];
        GetImage((int)Images.SkillIconImage).sprite = skillData.Icon;

        GetText((int)Texts.SkillNameText).SetText(skillData.Name);
        GetText((int)Texts.SkillLevelText).SetText($"Level : {skillLevel}");

        if (skillData is DurationSkillData durationSkillData)
        {
            string description = FormatSkillDescription(durationSkillData.Description, durationSkillData.GetSkillValue(skillLevel));
            GetText((int)Texts.SkillDescriptionText).SetText(description);
        }
        else if (skillData is ChargeSkillData chargeSkillData)
        {
            string description = FormatSkillDescription(chargeSkillData.Description, chargeSkillData.GetSkillValue(skillLevel));
            GetText((int)Texts.SkillDescriptionText).SetText(description);
        }

        SkillType = skillData.Type;
    }

    public void SetLocked(bool isLocked, string unlockDescription)
    {
        IsLocked = isLocked;
        var eventHandler = GetComponent<UIEventHandler>();
        if (eventHandler != null)
        {
            eventHandler.enabled = !isLocked;
        }

        if (isLocked)
        {
            GetText((int)Texts.SkillNameText).SetText("???");
            GetText((int)Texts.SkillDescriptionText).SetText(unlockDescription);
        }
    }

    private void OnSelectTab()
    {
        if (IsLocked)
        {
            return;
        }

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

    private static string FormatSkillDescription(string description, int value)
    {
        if (string.IsNullOrEmpty(description))
        {
            return string.Empty;
        }

        if (description.Contains("{0}"))
        {
            string coloredValue = $"<color=#{HighlightColorHex}>{value}</color>";
            return string.Format(description, coloredValue);
        }

        return description;
    }
}
