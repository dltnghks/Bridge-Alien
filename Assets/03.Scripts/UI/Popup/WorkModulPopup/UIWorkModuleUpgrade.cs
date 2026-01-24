using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkModuleUpgrade : UISubItem
{
    private const string HighlightColorHex = "69A96C";

    enum Images
    {
        BeforeSkillImage,
        AfterSkillImage,
    }

    enum Texts
    {
        BeforeSkillName,
        BeforeSkillLevel,
        BeforeSkillDescription,

        AfterSkillName,
        AfterSkillLevel,
        AfterSkillDescription,

        GoldText,
    }

    enum Buttons
    {
        UpgradeButton,
    }

    private Define.MiniGameSkillType _selectedSkillType;
    private SkillData _upgradeSkillData;
    private int _currentLevel;
    private int _upgradeGold;
    public Action OnClickUpdateAction;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);

        return true;
    }

    public void OnClickUpgradeButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());

        // 업그레이드 성공
        if (Managers.Player.UpgradeSkill(_selectedSkillType, _upgradeGold))
        {
            OnClickUpdateAction?.Invoke();
            return;
        }

        // 업그레이드 실패
        Logger.Log("업그레이드 실패");
    }

    public void SetInfo(Define.MiniGameSkillType skillType)
    {
        Init();

        _upgradeSkillData = Managers.Data.MiniGameSkillData.GetSkillData(skillType);
        _selectedSkillType = skillType;
        _currentLevel = Managers.Player.GetSkillLevel(_upgradeSkillData.Type);

        // 업그레이드
        if (_currentLevel >= _upgradeSkillData.GetMaxLevel())
        {
            SetMaxLevelSkillInfo();
            Logger.Log("업그레이드 불가");
            return;
        }

        _upgradeGold = _upgradeSkillData.UpgradeCostByLevel[_currentLevel];
    
        SetBeforeSkillInfo();
        SetAfterSkillInfo();

        GetText((int)Texts.GoldText).SetText($"{_upgradeGold}");
    }

    private void SetMaxLevelSkillInfo()
    {
        GetImage((int)Images.BeforeSkillImage).sprite = _upgradeSkillData.Icon;
        GetText((int)Texts.BeforeSkillName).SetText(_upgradeSkillData.Name);
        GetText((int)Texts.BeforeSkillLevel).SetText($"Level : {_currentLevel}");

        if (_upgradeSkillData is DurationSkillData durationSkillData)
        {
            string description = FormatSkillDescription(durationSkillData.Description, durationSkillData.GetSkillValue(_currentLevel));
            GetText((int)Texts.BeforeSkillDescription).SetText(description);
        }
        else if (_upgradeSkillData is ChargeSkillData chargeSkillData)
        {
            string description = FormatSkillDescription(chargeSkillData.Description, chargeSkillData.GetSkillValue(_currentLevel));
            GetText((int)Texts.BeforeSkillDescription).SetText(description);
        }

        GetImage((int)Images.AfterSkillImage).sprite = _upgradeSkillData.Icon;
        GetText((int)Texts.AfterSkillName).SetText(_upgradeSkillData.Name);
        GetText((int)Texts.AfterSkillLevel).SetText("MAX");
        GetText((int)Texts.AfterSkillDescription).SetText("최종 레벨에 도달했습니다.");

        GetText((int)Texts.GoldText).SetText("-");
    }

    private void SetBeforeSkillInfo()
    {
        GetImage((int)Images.BeforeSkillImage).sprite = _upgradeSkillData.Icon;
        GetText((int)Texts.BeforeSkillName).SetText(_upgradeSkillData.Name);
        GetText((int)Texts.BeforeSkillLevel).SetText($"Level : {_currentLevel}");

        if (_upgradeSkillData is DurationSkillData durationSkillData)
        {
            string description = FormatSkillDescription(durationSkillData.Description, durationSkillData.GetSkillValue(_currentLevel));
            GetText((int)Texts.BeforeSkillDescription).SetText(description);
        }
        else if (_upgradeSkillData is ChargeSkillData chargeSkillData)
        {
            string description = FormatSkillDescription(chargeSkillData.Description, chargeSkillData.GetSkillValue(_currentLevel));
            GetText((int)Texts.BeforeSkillDescription).SetText(description);
        }
    }


    private void SetAfterSkillInfo()
    {
        int upgradeLevel = _currentLevel + 1;
        GetImage((int)Images.AfterSkillImage).sprite = _upgradeSkillData.Icon;
        GetText((int)Texts.AfterSkillName).SetText(_upgradeSkillData.Name);
        GetText((int)Texts.AfterSkillLevel).SetText($"Level : {upgradeLevel}");

        if (_upgradeSkillData is DurationSkillData durationSkillData)
        {
            string description = FormatSkillDescription(durationSkillData.Description, durationSkillData.GetSkillValue(upgradeLevel));
            GetText((int)Texts.AfterSkillDescription).SetText(description);
        }
        else if (_upgradeSkillData is ChargeSkillData chargeSkillData)
        {
            string description = FormatSkillDescription(chargeSkillData.Description, chargeSkillData.GetSkillValue(upgradeLevel));
            GetText((int)Texts.AfterSkillDescription).SetText(description);
        }

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
