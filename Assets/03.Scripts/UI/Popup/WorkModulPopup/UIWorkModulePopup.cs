using System.Collections.Generic;
using UnityEngine;

public class UIWorkModulePopup : UIPopup
{

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

        OnClickGameTypeButton(Define.MiniGameType.Unload);
        
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
            ApplySkillLockState(_workModuleSkillList[i], skillTypeList[i]);
        }

        if (_currentSelectedSkillButton != null && _currentSelectedSkillButton.IsLocked == false)
        {
            _upgradeUI.SetInfo(_selectedSkillType);
        }
    }

    private void OnClickGameTypeButton(Define.MiniGameType gameType)
    {
        if (gameType == Define.MiniGameType.Unknown)
        {
            gameType = Define.MiniGameType.Unload;
        }

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
            ApplySkillLockState(_workModuleSkillList[i], skillTypeList[i]);
            _workModuleSkillList[i].gameObject.SetActive(true);
        }

        UIWorkModuleSkillButton firstUnlocked = null;
        for (int i = 0; i < _workModuleSkillList.Count; i++)
        {
            if (_workModuleSkillList[i].gameObject.activeSelf && _workModuleSkillList[i].IsLocked == false)
            {
                firstUnlocked = _workModuleSkillList[i];
                break;
            }
        }

        if (firstUnlocked != null)
        {
            _currentSelectedSkillButton = firstUnlocked;
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

    private void ApplySkillLockState(UIWorkModuleSkillButton button, SkillData skillData)
    {
        if (TryGetUnlockStage(skillData.Type, out var stageType, out string stageText))
        {
            bool isLocked = Managers.Player.GetStageClearInfo(stageType) <= 0;
            string unlockDescription = $"해금 조건 : {stageText} 스테이지 최초 클리어";
            button.SetLocked(isLocked, unlockDescription);
            return;
        }

        button.SetLocked(false, string.Empty);
    }

    private bool TryGetUnlockStage(Define.MiniGameSkillType skillType, out Define.ChapterType stageType, out string stageText)
    {
        stageType = default;
        stageText = string.Empty;

        switch (skillType)
        {
            case Define.MiniGameSkillType.BoxWarpSkill:
                stageType = Define.ChapterType.CH2;
                stageText = "1-2";
                return true;
            case Define.MiniGameSkillType.CoolingSkill:
                stageType = Define.ChapterType.CH3;
                stageText = "1-3";
                return true;
            default:
                return false;
        }
    }
}
