using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITaskTabButton : UISubItem
{
    enum Texts
    {
        ButtonText,
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
        
        gameObject.BindEvent(OnSelectTab);
        
        return true;
    }
    
    public void Init(UIPlayerTaskPopup tabController, Define.TaskType type)
    {
        _tabController = tabController;
        _taskType = type;
    }
    
    private void OnSelectTab()
    {
        _tabController.SelectButton(this);
    }
    
    public void Select()
    {
        GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
    }

    public void Deselect()
    {        
        GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }
}
