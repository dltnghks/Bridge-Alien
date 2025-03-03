using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyManager
{
    private int _curDate;
    private int _dueDate = 14;
    
    private DailyData _currentDailyData;

    public void Init()
    {
        _curDate = 0;
    }
    
    // 일차 진행
    public void AddDate()
    {
        _curDate++;
        
        // 오늘자 데이터 세팅
        SetDailyData();
    }

    private void SetDailyData()
    {
        // DataManager에서 curDate 세팅 값 가져오기
        _currentDailyData = Managers.Data.DailyDataManager.GetData(_curDate);

        StartEvent();
    }
    
    // 이벤트 처리
    private void StartEvent()
    {
        if (_currentDailyData.EventType != Define.DialogType.Unknown)
        {
            Managers.UI.ShowPopUI<UIDialogPopup>().SetDialogs(_currentDailyData.EventType, EndEvent);    
        }
        else
        {
            EndEvent();
        }
    }

    private void EndEvent()
    {
        switch (_currentDailyData.MiniGameType)
        {
            case Define.MiniGameType.Unload:
                Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload); break;
            case Define.MiniGameType.Delivery:
                Managers.Scene.ChangeScene(Define.Scene.MiniGameDelivery); break;
            default:
                Logger.LogError("Not Find MiniGame"); break;
        }
    }
    
}
