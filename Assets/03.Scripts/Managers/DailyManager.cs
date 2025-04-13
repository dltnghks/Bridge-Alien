using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyManager
{
    private int _curDate;
    private readonly int _dueDate = 3;
    
    private DailyData _currentDailyData;
    private Dictionary<string, DailyData> _currentDailyDataDict = new Dictionary<string, DailyData>();

    private UIDialogPopup _dialogPopup = null;

    public int CurrentDate
    {
        get{return _curDate;}
    }
    public DailyData CurrentDailyData
    {
        get { return _currentDailyData; }
    }


    public void Init(int lastDate = 1, DailyData dailyData = null)
    {
        int initData = lastDate;
        SetCurrentData(initData);
        SetDailyData(dailyData);
    }

    private void SetCurrentData(int value)
    {
        _curDate = value;
        
        Logger.Log($"진행일 : {_curDate}");
    }
    
    private void AddCurrentData(int value)
    {
        _curDate += value;
    }
    
    // 일차 진행
    public void AddDate()
    {
        AddCurrentData(1);

        if (_curDate >= _dueDate)
        {
            EndGame();
            return;
        }
        
        // 플레이어 일차 변화
        Managers.Player.AddDate();
        
        // 오늘자 데이터 세팅
        SetDailyData();
        StartEvent();
    }

    public void SetDailyData(DailyData dailyData = null)
    {
        // DataManager에서 curDate 세팅 값 가져오기
        _currentDailyDataDict = Managers.Data.DailyData.GetData("Day" + _curDate);
        if (_currentDailyDataDict == null)
        {
            Logger.LogError("currentDailyDataDict is null");
        }
        
        
        if (dailyData == null && _currentDailyDataDict.ContainsKey("Start"))
        {
            dailyData = _currentDailyDataDict["Start"];
        }
        
        // 시작 이벤트 구분하기
        _currentDailyData = dailyData;
    }
    
    public void NextEvent()
    {
        if (_currentDailyDataDict.ContainsKey(_currentDailyData.NextEventID))
        {
            _currentDailyData = _currentDailyDataDict[_currentDailyData.NextEventID];
        }
        else
        {
            // 다음 이벤트가 없으면(= 하루가 끝났다면)
            _currentDailyData = null;
            return;
        }
        
        switch (_currentDailyData.Time)
        {
            case 0:
                Logger.Log("daily time is 0");
                break;
            case 1:
                Logger.Log("daily time is 1");
                break;
            case 2:
                Logger.Log("daily time is 2");
                break;
            default:
                Logger.Log("daily time is null");
                break;
        }
        
        if (_currentDailyData.EventType == Define.DailyEventType.End)
        {
            EndDay();
            return;
        }
        
        StartEvent();
    }
    
    // 이벤트 처리
    private void StartEvent()
    {
        Logger.Log($"StartEvent {_currentDailyData.EventID}");
        if (_currentDailyData.EventType == Define.DailyEventType.Dialog)
        {
            Logger.Log($"Daily Event is Dialog : {_currentDailyData.GetParameter<Define.Dialog>()}");
            
            if(_dialogPopup != null) _dialogPopup.ClosePopupUI();

            _dialogPopup = Managers.UI.ShowPopUI<UIDialogPopup>(); 
            _dialogPopup.SetDialogs(_currentDailyData.GetParameter<Define.Dialog>(), NextEvent);    
        }
        else if(_currentDailyData.EventType == Define.DailyEventType.MiniGame)
        {
            StartMiniGameEvent();
        }
        
    }

    private void StartMiniGameEvent()
    {
        if (_currentDailyData.EventType != Define.DailyEventType.MiniGame)
        {
            Logger.LogError("EventType is not MiniGame");
            return;
        }
        
        switch (_currentDailyData.GetParameter<Define.MiniGameType>())
        {
            case Define.MiniGameType.Unload:
                Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload); break;
            case Define.MiniGameType.Delivery:
                Managers.Scene.ChangeScene(Define.Scene.MiniGameDelivery); break;
            default:
                Logger.LogError("Not Find MiniGame"); break;
        }   
    }

    public void EndMiniGameEvent()
    {
        Managers.Scene.ChangeScene(Define.Scene.House);
    }
    
    private void EndDay()
    {
        AddDate();
    }

    private void EndGame()
    {
        Logger.Log("EndGame");
        Managers.Scene.ChangeScene(Define.Scene.Ending);
    }
}
