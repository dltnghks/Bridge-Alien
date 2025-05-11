using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHouseScene : UIScene
{
    enum Texts{
        DayText,
        TimeText,
        GoldText,
        FatigueText,
    }

    enum Buttons
    {
        PlayerStatusButton,
        TaskButton,
        NextButton,
    }

    enum Objects
    {
        UIFatigue,
    }
    
    private UIPopup _currentPopup = null;
    private Slider _fatigueSlider;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));
        
        _fatigueSlider = GetObject((int)Objects.UIFatigue).GetOrAddComponent<Slider>();
        
        SetDayText();
        SetTimeText();
        
        GetButton((int)Buttons.PlayerStatusButton).gameObject.BindEvent(OnClickPlayerStatusButton);
        GetButton((int)Buttons.TaskButton).gameObject.BindEvent(OnClickTaskButton);

        SetGoldText();
        SetFatigue();
        
        GetButton((int)Buttons.NextButton).gameObject.BindEvent(OnClickNextButton);
        
        return true;
    }

    public override void UIUpdate()
    {
        base.UIUpdate();
        SetDayText();
        SetTimeText();
    }

    private void OnClickPlayerStatusButton()
    {
        if (_currentPopup)
        {
            _currentPopup.ClosePopupUI();
            return;
        }
        
        _currentPopup = Managers.UI.ShowPopUI<UIPlayerStatusPopup>();
    }

    private void OnClickTaskButton()
    {
        // 일과 팝업 생성
        if (_currentPopup)
        {
            _currentPopup.ClosePopupUI();
            return;
        }
        
        _currentPopup = Managers.UI.ShowPopUI<UIPlayerTaskPopup>();
    }
    
    // Test Code
    private void OnClickNextButton()
    {
        Managers.Daily.StartEvent();
    }


    private void SetDayText()
    {
        GetText((int)Texts.DayText).SetText($"Day {Managers.Daily.CurrentDate}");
    }
    
    private void SetTimeText()
    {
        GetText((int)Texts.TimeText).SetText($"{Managers.Daily.CurrentDailyData.Time}");
    }
    
    private void SetGoldText()
    {
        GetText((int)Texts.GoldText).text = $"{Managers.Player.GetGold()}N";
    }
    
    private void SetFatigue()
    {
        int curFatigue = Managers.Player.GetStat(Define.PlayerStatType.Fatigue);
        GetText((int)Texts.FatigueText).text = $"{curFatigue}/100";
        _fatigueSlider.value = (float)curFatigue / 100f;
    }

    private void OnEnable()
    {
        Managers.Player.OnPlayerDataChanged += SetGoldText;
        Managers.Player.OnPlayerDataChanged += SetFatigue;
    }
    
    private void OnDisable()
    {
        Managers.Player.OnPlayerDataChanged -= SetGoldText;
        Managers.Player.OnPlayerDataChanged -= SetFatigue;
    }
}
