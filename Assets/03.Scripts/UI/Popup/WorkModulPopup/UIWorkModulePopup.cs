using System.Collections.Generic;
using UnityEngine;

public class UIWorkModulePopup : UIPopup
{
    enum Buttons
    {
        UnloadButton,
        DeliveryButton,
    }

    enum Objects
    {
        SkillGroup,
        UIWorkModuleUpgrade,
    }

    private List<UIWorkModuleSkillButton> _workModuleSkillList = new List<UIWorkModuleSkillButton>();
    private UIWorkModuleSkillButton _currentSelectedSkillButton = null;
    private Define.MiniGameSkillType _selectedSkillType;
    private Define.MiniGameType _selectedGameType = Define.MiniGameType.Unload;

    private UIWorkModuleUpgrade _upgradeUI;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        _upgradeUI = GetObject((int)Objects.UIWorkModuleUpgrade).GetOrAddComponent<UIWorkModuleUpgrade>();
        _upgradeUI.OnClickUpdateAction = UpdateUI;

        _workModuleSkillList.Clear();

        GameObject skillGroup = GetObject((int)Objects.SkillGroup);
        foreach (Transform child in skillGroup.transform)
        {
            if (child.TryGetComponent<UIWorkModuleSkillButton>(out var skillButton))
            {
                _workModuleSkillList.Add(skillButton);
            }
        }

        GetButton((int)Buttons.UnloadButton).gameObject.BindEvent(() => OnClickGameTypeButton(Define.MiniGameType.Unload));
        GetButton((int)Buttons.DeliveryButton).gameObject.BindEvent(() => OnClickGameTypeButton(Define.MiniGameType.Delivery));

        foreach (var button in _workModuleSkillList)
        {
            button.Init();
        }


        // 팝업이 열렸을 때, 하차게임으로 세팅
        _selectedGameType = Define.MiniGameType.Unload;
        InitSkillList();

        return true;
    }

    private void UpdateUI()
    {
        var skillTypeList = Managers.Data.MiniGameSkillData.GetMiniGameSkillList(_selectedGameType);
        for (int i = 0; i < skillTypeList.Count; i++)
        {
            _workModuleSkillList[i].SetWorkModuleSkillInfo(skillTypeList[i]);
        }
        _upgradeUI.SetInfo(_selectedSkillType);
    }

    private void OnClickGameTypeButton(Define.MiniGameType gameType)
    {
        if (gameType == Define.MiniGameType.Unknown)
        {
            gameType = Define.MiniGameType.Unload;
        }

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _selectedGameType = gameType;
        InitSkillList();
    }

    private void InitSkillList()
    {
        var skillTypeList = Managers.Data.MiniGameSkillData.GetMiniGameSkillList(_selectedGameType);

        // Disable unused skill UI elements
        foreach (var workModuleSkill in _workModuleSkillList)
        {
            workModuleSkill.Deselect();
            workModuleSkill.gameObject.SetActive(false);
        }

        // Set info for each skill UI element
        for (int i = 0; i < skillTypeList.Count; i++)
        {
            _workModuleSkillList[i].SetWorkModuleSkillInfo(skillTypeList[i]);
            _workModuleSkillList[i].Init(this);
            _workModuleSkillList[i].gameObject.SetActive(true);
        }

        if (_workModuleSkillList.Count > 0)
        {
            _currentSelectedSkillButton = _workModuleSkillList[0];
            SelectSkillButton(_currentSelectedSkillButton);
        }
        else
        {
            _currentSelectedSkillButton?.Deselect();
            _currentSelectedSkillButton = null;
        }
    }

    public void SelectSkillButton(UIWorkModuleSkillButton skillButton)
    {
        if (_currentSelectedSkillButton != null)
        {
            _currentSelectedSkillButton.Deselect();
        }

        _currentSelectedSkillButton = skillButton;
        _selectedSkillType = _currentSelectedSkillButton.SkillType;

        _currentSelectedSkillButton.Select();
        _upgradeUI.SetInfo(_selectedSkillType);
    }
}
