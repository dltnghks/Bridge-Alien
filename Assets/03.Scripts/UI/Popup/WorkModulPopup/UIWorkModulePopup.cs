using System.Collections.Generic;
using UnityEngine;

public class UIWorkModulePopup : UIPopup
{
    enum Buttons
    {
        UnloadButton,
        DeliveryButton,
        UpgradeButton,
    }

    enum Objects
    {
        SkillGroup,
        UIWorkModuleSkill,
    }

    private List<UIWorkModuleSkillButton> _workModuleSkillList = new List<UIWorkModuleSkillButton>();
    private Transform _skillGroupTransform;
    private GameObject _workModuleSkillTemplate;
    private UIWorkModuleSkillButton _currentSelectedSkillButton = null;
    private Define.MiniGameSkillType _selectedSkillType;
    private Define.MiniGameType _selectedGameType = Define.MiniGameType.Unload;
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        _skillGroupTransform = GetObject((int)Objects.SkillGroup).transform;
        _workModuleSkillTemplate = GetObject((int)Objects.UIWorkModuleSkill);
        _workModuleSkillTemplate.SetActive(false); // Disable the template

        GetButton((int)Buttons.UnloadButton).gameObject.BindEvent(OnClickUnloadButton);
        GetButton((int)Buttons.DeliveryButton).gameObject.BindEvent(OnClickDeliveryButton);
        GetButton((int)Buttons.UpgradeButton).gameObject.BindEvent(OnClickUpgradeButton);

        // 팝업이 열렸을 때, 하차게임으로 세팅
        OnClickUnloadButton();

        return true;
    }

    private void OnClickUnloadButton()
    {
        _selectedGameType = Define.MiniGameType.Unload;
        InitSkillList();
    }

    private void OnClickDeliveryButton()
    {
        _selectedGameType = Define.MiniGameType.Delivery;
        InitSkillList();
    }

    private void OnClickUpgradeButton()
    {
        if (_currentSelectedSkillButton is null)
            return;

        // 해당 스킬이 더 이상 업그레이드가 불가능하면 return
        int currentSkillLevel = Managers.Player.GetSkillLevel(_selectedSkillType);
        int maxLevel = Managers.Data.MiniGameSkillData.GetSkillData(_selectedSkillType).GetMaxLevel();
        Logger.Log(currentSkillLevel + ", " + maxLevel);
        if (currentSkillLevel >= maxLevel)
        {
            return;
        }

            _currentSelectedSkillButton.Deselect();
        _currentSelectedSkillButton = null;

        UIWorkModuleUpgradePopup workModuleUpgradePopup = Managers.UI.ShowPopUI<UIWorkModuleUpgradePopup>();
        workModuleUpgradePopup.SetInfo(_selectedSkillType, InitSkillList);
    }

    private void InitSkillList(){

        Managers.UI.SetInputBackground(true);

        switch (_selectedGameType)
        {
            case Define.MiniGameType.Unload:
                SetSkillList(Managers.Data.MiniGameSkillData.GetMiniGameSkillList(Define.MiniGameType.Unload));
                break;
            case Define.MiniGameType.Delivery:
                SetSkillList(Managers.Data.MiniGameSkillData.GetMiniGameSkillList(Define.MiniGameType.Delivery));
                break;
            default:
                return;
        }
    }

    private void SetSkillList(List<SkillData> skillTypeList)
    {
        _currentSelectedSkillButton?.Deselect();
        _currentSelectedSkillButton = null;

        // Disable unused skill UI elements
        foreach(var workModuleSkill in _workModuleSkillList)
        {
            workModuleSkill.gameObject.SetActive(false);
        }

        // Adjust the number of skill UI elements to match the skill list
        while (_workModuleSkillList.Count < skillTypeList.Count)
        {
            AddWorkModuleSkillButton();
        }

        // Set info for each skill UI element
        for (int i = 0; i < skillTypeList.Count; i++)
        {
            _workModuleSkillList[i].SetWorkModuleSkillInfo(skillTypeList[i]);
            _workModuleSkillList[i].Init(this);
            _workModuleSkillList[i].gameObject.SetActive(true);
        }
    }

    private void AddWorkModuleSkillButton()
    {
        GameObject newWorkModuleSkillObj = Managers.Resource.Instantiate(_workModuleSkillTemplate, _skillGroupTransform);
        UIWorkModuleSkillButton newWorkModuleSkill = newWorkModuleSkillObj.GetComponent<UIWorkModuleSkillButton>();
        newWorkModuleSkill.Init(this);
        _workModuleSkillList.Add(newWorkModuleSkill);
    }
    
    public void SelectSkillButton(UIWorkModuleSkillButton taskButton)
    {
        // 현재 고른 Task 변경
        if (_currentSelectedSkillButton != null)
        {
            _currentSelectedSkillButton.Deselect();
        }

        _currentSelectedSkillButton = taskButton;
        _selectedSkillType = _currentSelectedSkillButton.SkillType;
        
        _currentSelectedSkillButton.Select();
    }
}
