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

    [SerializeField] private Sprite _activeButtonSprite;
    [SerializeField] private Sprite _dectiveButtonSprite;
    
    private Define.TaskType _taskType;
    private UIPlayerTaskPopup _tabController;
    private Image _buttonImage;

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
        _buttonImage.sprite = _activeButtonSprite;
    }

    public void Deselect()
    {        
        _buttonImage.sprite = _dectiveButtonSprite;
    }
}
