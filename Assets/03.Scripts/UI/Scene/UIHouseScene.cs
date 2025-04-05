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
    }

    enum Images
    {
        PopupBackground,
    }

    private UIPlayerStatusPopup _uIPlayerStatusPopup = null;
    
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
        
        return true;
    }

    private void OnClickPlayerStatusButton()
    {
        if (_uIPlayerStatusPopup)
        {
            _uIPlayerStatusPopup.ClosePopupUI();
            return;
        }
        
        _uIPlayerStatusPopup = Managers.UI.ShowPopUI<UIPlayerStatusPopup>();
    }

    private void OnClickTaskButton()
    {
        // 일과 팝업 생성
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
