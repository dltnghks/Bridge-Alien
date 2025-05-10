using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITaskTabButton : UIActiveButton
{
    enum Texts
    {
        ButtonText,
    }

    enum Objects
    {
        Flag,
    }

    private Define.TaskType _taskType;
    private UIPlayerTaskPopup _tabController;

    public Define.TaskType TaskType
    {
        get{ return _taskType; }
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindObject(typeof(Objects));
        
        gameObject.BindEvent(OnSelectTab);
        
        return true;
    }
    
    public void Init(UIPlayerTaskPopup tabController, Define.TaskType type)
    {
        Init();
        
        _tabController = tabController;
        _taskType = type;
        _buttonImage = GetComponent<Image>();
        
        Deselect();
    }
    
    private void OnSelectTab()
    {
        _tabController.SelectTabButton(this);
    }
    
    public void Select()
    {
        Activate();
        GetObject((int)Objects.Flag).SetActive(true);
    }

    public void Deselect()
    {        
        Deactivate();
        GetObject((int)Objects.Flag).SetActive(false);
    }
}
