using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyManager
{
    private int[] _dateList = { 1, 2, 3, 4, 11, 14, 15 };
    private int _currentIndex = 0;
    private int _curDate;
    private readonly int _dueDate = 16;
    
    private DailyData _preDailyData;
    private DailyData _currentDailyData;
    private Dictionary<string, DailyData> _currentDailyDataDict = new Dictionary<string, DailyData>();

    private UIDialogPopup _dialogPopup = null;

    public int CurrentDate => _curDate;
    public DailyData PreDailyData => _preDailyData;
    public DailyData CurrentDailyData => _currentDailyData;


    public void Init(int lastDate = 1, DailyData dailyData = null)
    {
        Logger.Log("Initializing daily data");
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
        _currentIndex += value;
        _curDate = _dateList[_currentIndex];
    }
    
    // 일차 진행
    public void AddDate()
    {
        Logger.Log("AddDate");
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
        
        Managers.Scene.ChangeScene(Define.Scene.House); 
    }

    private void SetDailyData(DailyData dailyData = null)
    {
        Logger.Log("SetDailyData");
        // DataManager에서 curDate 세팅 값 가져오기
        _currentDailyDataDict = Managers.Data.DailyData.GetData("Event_D" + _curDate); 
        if (_currentDailyDataDict == null)
        {
            Logger.LogError("currentDailyDataDict is null");
            return;
        }
        
        // 시작 이벤트 설정
        if (dailyData == null)
        {
            string startStr = "Start";
            // 선택지를 고른 경우, 시작 데이터를 start+선택지 번호로
            if (Managers.Player.PlayerData.ChoiceNumber >= 0)
            {
                startStr += Managers.Player.PlayerData.ChoiceNumber.ToString();
            }

            if (_currentDailyDataDict.ContainsKey(startStr))
            {
                dailyData = _currentDailyDataDict[startStr];
            }
            else
            {
                Logger.LogError($"{startStr} Daily data is empty");
            }
        }
        
        // 시작 이벤트 구분하기
        _currentDailyData = dailyData;
    }
    
    private void SetNextEvent()
    {
        if (_currentDailyDataDict.ContainsKey(_currentDailyData.NextEventID))
        {
            _preDailyData = _currentDailyData; 
            _currentDailyData = _currentDailyDataDict[_currentDailyData.NextEventID];
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
        
    }

    // 이벤트 처리
    public void StartEvent()
    {
        Managers.UI.SceneUI.UIUpdate();

        if (_currentDailyData == null)
        {
            Logger.LogError("currentDailyData is null");
            return;
        }

        Logger.Log($"StartEvent | Time : {_currentDailyData.Time}, ID : {_currentDailyData.EventID}");

        if (_currentDailyData.EventType == Define.DailyEventType.Unknown)
        {
            // 처음인 경우 바로 다음으로 넘어가기
            SetNextEvent();
            StartEvent();
            //Managers.Scene.ChangeScene(Define.Scene.House);
            return;
        }

        if (_currentDailyData.EventType == Define.DailyEventType.Dialog)
        {
            Logger.Log($"Daily Event is Dialog : {_currentDailyData.GetParameter<Define.Dialog>()}");

            _dialogPopup = Managers.UI.ShowPopUI<UIDialogPopup>();
            _dialogPopup.InitDialog(_currentDailyData.GetParameter<Define.Dialog>(), _currentDailyData.DialogScene, StartEvent);
        }
        else if (_currentDailyData.EventType == Define.DailyEventType.MiniGame)
        {
            StartMiniGameEvent();
        }
        else if (_currentDailyData.EventType == Define.DailyEventType.End)
        {
            EndDay();
            return;
        }

        SetNextEvent();
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

