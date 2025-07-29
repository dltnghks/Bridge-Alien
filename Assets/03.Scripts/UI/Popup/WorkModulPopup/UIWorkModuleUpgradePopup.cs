using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorkModuleUpgradePopup : UIPopup
{
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
        CancelButton,
    }

    private Define.MiniGameSkillType _selectedSkillType;
    private SkillData _upgradeSkillData;
    private int _currentLevel;
    private int _upgradeGold;
    private Action _callback;

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
        GetButton((int)Buttons.CancelButton).gameObject.BindEvent(OnClickCancelButton);

        return true;
    }

    private void OnClickUpgradeButton()
    {

        int upgradeGold = _upgradeSkillData.UpgradeCostByLevel[_currentLevel];

        // 업그레이드 성공
        if (Managers.Player.UpgradeSkill(_selectedSkillType, upgradeGold))
        {
            ClosePopupUI();
            return;
        }

        // 업그레이드 실패
        Logger.Log("업그레이드 실패");

    }

    private void OnClickCancelButton()
    {
        ClosePopupUI();
    }

    public void SetInfo(Define.MiniGameSkillType skillType, Action callback = null)
    {
        Init();

        _upgradeSkillData = Managers.Data.MiniGameSkillData.GetSkillData(skillType);
        _selectedSkillType = skillType;
        _currentLevel = Managers.Player.GetSkillLevel(_upgradeSkillData.Type);
        _upgradeGold = _upgradeSkillData.UpgradeCostByLevel[_currentLevel];
        _callback = callback;

        SetBeforeSkillInfo();
        SetAfterSkillInfo();

        GetText((int)Texts.GoldText).SetText($"{_upgradeGold}");
    }

    private void SetBeforeSkillInfo()
    {
        GetImage((int)Images.BeforeSkillImage).sprite = _upgradeSkillData.Icon;
        GetText((int)Texts.BeforeSkillName).SetText(_upgradeSkillData.Name);
        GetText((int)Texts.BeforeSkillLevel).SetText($"Level : {_currentLevel}");

        if (_upgradeSkillData is DurationSkillData durationSkillData)
        {
            GetText((int)Texts.BeforeSkillDescription).SetText(string.Format(durationSkillData.Description, durationSkillData.GetSkillValue(_currentLevel)));
        }
        else if (_upgradeSkillData is ChargeSkillData chargeSkillData)
        {
            GetText((int)Texts.BeforeSkillDescription).SetText(string.Format(chargeSkillData.Description, chargeSkillData.GetSkillValue(_currentLevel)));
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
            GetText((int)Texts.AfterSkillDescription).SetText(string.Format(durationSkillData.Description, durationSkillData.GetSkillValue(upgradeLevel)));
        }
        else if (_upgradeSkillData is ChargeSkillData chargeSkillData)
        {
            GetText((int)Texts.AfterSkillDescription).SetText(string.Format(chargeSkillData.Description, chargeSkillData.GetSkillValue(upgradeLevel)));
        }

    }


    public override void ClosePopupUI()
    {
        _callback?.Invoke();
        base.ClosePopupUI();
    }
}
