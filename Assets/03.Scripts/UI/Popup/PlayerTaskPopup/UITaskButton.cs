using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITaskButton : UISubItem
{
    enum Texts
    {
        TaskButtonText,
        GoldText,
    }
    
    private UIPlayerTaskPopup _tabController;
    private Outline _outline;
    public PlayerTaskData PlayerTaskData { get; private set; }

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
    
    public void Init(UIPlayerTaskPopup tabController)
    {
        Init();
        
        _outline = GetComponent<Outline>();
        _tabController = tabController;
        
        Deselect();
    }
    
    private void OnSelectTab()
    {
        _tabController.SelectTaskButton(this);
    }
    
    public void SetData(PlayerTaskData data)
    {
        GetText((int)Texts.TaskButtonText).text = data.TaskName;
        GetText((int)Texts.GoldText).text = data.RequirementGold.ToString();
        PlayerTaskData = data;
    }
    
    public void Select()
    {
        _outline.enabled = true;
    }

    public void Deselect()
    {
        _outline.enabled = false;
    }
}
