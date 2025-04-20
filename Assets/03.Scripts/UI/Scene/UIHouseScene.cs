using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHouseScene : UIScene
{
    enum Texts{
        DayText,
        TimeText,
    }

    enum Buttons
    {
        PlayerStatusButton,
        TaskButton,
        
        // Test Code
        SaveButton,
        LoadButton,
        NextButton,
    }

    enum Images
    {
        PopupBackground,
    }

    private UIPopup _currentPopup = null;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        
        SetDayText();
        SetTimeText();
        
        GetButton((int)Buttons.PlayerStatusButton).gameObject.BindEvent(OnClickPlayerStatusButton);
        GetButton((int)Buttons.TaskButton).gameObject.BindEvent(OnClickTaskButton);
        
        // Test Code
        GetButton((int)Buttons.SaveButton).gameObject.BindEvent(OnClickSaveButton);
        GetButton((int)Buttons.LoadButton).gameObject.BindEvent(OnClickLoadButton);
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
    private void OnClickSaveButton()
    {
        Managers.Save.Save();
    }

    // Test Code
    private void OnClickLoadButton()
    {
        Managers.Save.Load();
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
}
