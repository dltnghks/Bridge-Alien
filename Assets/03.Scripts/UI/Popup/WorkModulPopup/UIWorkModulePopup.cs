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
        UIWorkModuleSkill,
    }

    private List<UIWorkModuleSkillButton> _workModuleSkillList = new List<UIWorkModuleSkillButton>();
    private Transform _skillGroupTransform;
    private GameObject _workModuleSkillTemplate;

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

        // 팝업이 열렸을 때, 하차게임으로 세팅
        OnClickUnloadButton();

        return true;
    }

    private void OnClickUnloadButton()
    {
        SetSkillList(Managers.Data.MiniGameSkillData.GetMiniGameSkillList(Define.MiniGameType.Unload));
    }

    private void OnClickDeliveryButton()
    {
        SetSkillList(Managers.Data.MiniGameSkillData.GetMiniGameSkillList(Define.MiniGameType.Delivery));
    }


    private void SetSkillList(List<SkillData> skillTypeList)
    {
        // Adjust the number of skill UI elements to match the skill list
        while (_workModuleSkillList.Count < skillTypeList.Count)
        {
            AddWorkModuleSkillButton();
        }

        // Set info for each skill UI element
        for (int i = 0; i < skillTypeList.Count; i++)
        {
            _workModuleSkillList[i].SetWorkModuleSkillInfo(skillTypeList[i]);
            _workModuleSkillList[i].gameObject.SetActive(true);
        }
        
        // Disable unused skill UI elements
        for (int i = skillTypeList.Count; i < _workModuleSkillList.Count; i++)
        {
            _workModuleSkillList[i].gameObject.SetActive(false);
        }
    } 
    
    private void AddWorkModuleSkillButton()
    {
        GameObject newWorkModuleSkillObj = Managers.Resource.Instantiate(_workModuleSkillTemplate, _skillGroupTransform);
        UIWorkModuleSkillButton newWorkModuleSkill = newWorkModuleSkillObj.GetComponent<UIWorkModuleSkillButton>();
        newWorkModuleSkill.Init();
        _workModuleSkillList.Add(newWorkModuleSkill);
    }
}
