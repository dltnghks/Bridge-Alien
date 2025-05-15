using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITaskButton : UIActiveButton
{
    enum Texts
    {
        TaskButtonText,
        GoldText,
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
        
        Deselect();
        
        return true;
    }
    
    public void Init(UIPlayerTaskPopup tabController)
    {
        Init();
        
        _tabController = tabController;
    }
    
    private void OnSelectTab()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
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
        Activate();
    }

    public void Deselect()
    {
        Deactivate();
    }
}
