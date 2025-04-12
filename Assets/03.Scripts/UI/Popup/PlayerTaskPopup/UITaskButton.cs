using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITaskButton : UISubItem
{
    enum Texts
    {
        TaskButtonText,
    }
    
    private UIPlayerTaskPopup _tabController;
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
        _tabController = tabController;
    }
    
    private void OnSelectTab()
    {
        _tabController.SelectTaskButton(this);
    }
    
    public void SetData(PlayerTaskData data)
    {
        GetText((int)Texts.TaskButtonText).text = data.TaskName;
        PlayerTaskData = data;
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
