using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : ISaveable
{
    private string _lastEventID;
    private EventData _curEventData;
    private Dictionary<string, EventData> _currentEventDataDict = new Dictionary<string, EventData>();
    public EventData CurEventData => _curEventData;
    private UIDialogPopup _dialogPopup = null;

    public void Init(string lastEventID = null)
    {
        _lastEventID = lastEventID;
        SetEventData(_lastEventID);
    }

    private void SetEventData(string lastEventID = null)
    {
        // DataManager에서 curDate 세팅 값 가져오기
        int curStageNum = Managers.Player.GetCleardStageNum() + 1;
        _currentEventDataDict = Managers.Data.EventData.GetData($"Event_C{curStageNum}");

        if (_currentEventDataDict == null)
        {
            Logger.LogError("CurrentEventDataDict is null");
            return;
        }

        // 시작 이벤트 설정
        if (lastEventID == null)
        {
            lastEventID = "Start";
        }

        // 현재 이벤트 설정
        if (_currentEventDataDict.ContainsKey(lastEventID))
        {
            _curEventData = _currentEventDataDict[lastEventID];
        }
        else
        {
            Logger.LogError($"{lastEventID} Event data is empty");
        }

        // 시작 이벤트는 바로 넘어가기
        if (lastEventID == "Start")
        {
            PlayEvent();
        }
        
    }

    public void SetNextEventData()
    {
        string nextEventID = _curEventData.NextEventID;
        if (_currentEventDataDict.ContainsKey(nextEventID))
        {
            _curEventData = _currentEventDataDict[nextEventID];
        }
        else
        {
            Logger.LogError($"{nextEventID} is empty");
        }
    }

    public void PlayEvent()
    {

        // check event type
        if (_curEventData.EventType == Define.EventType.Dialog)
        {
            PlayDialog();
        }
        else if (_curEventData.EventType == Define.EventType.MiniGame)
        {
            // 게임을 클리어했을 때만 다음 이벤트 진행
            PlayGame();
            return;
        }
        else if (_curEventData.EventType == Define.EventType.End)
        {
            EndEvent();
            return;
        }
        else if (_curEventData.EventType == Define.EventType.Unknown)
        {
            
        }
        else
        {
            // type set error
            Logger.LogError($"Error : {_curEventData.EventType} is not define");
            EndEvent();
            return;
        }

        SetNextEventData();
    }

    private void PlayDialog()
    {
        Define.Dialog dialogID = _curEventData.GetParameter<Define.Dialog>();

        // 대화 이벤트가 연속되는 경우, 데이터만 교체
        if (_dialogPopup == null)
            _dialogPopup = Managers.UI.ShowPopUI<UIDialogPopup>();

        _dialogPopup.InitDialog(dialogID, _curEventData.DialogScene, PlayEvent);
    }

    private void PlayGame()
    {
        Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload);
    }

    public void EndEvent()
    {
        Managers.Scene.ChangeScene(Define.Scene.House);

        // 다음 이벤트 데이터 세팅
        SetEventData();
    }

    public void Add(ISaveable saveable)
    {
        throw new NotImplementedException();
    }

    // 현재 상태를 저장 가능한 객체로 캡처하여 반환
    public object CaptureState()
    {
        var data = new EventSaveData();
        return data;
    }

    // 캡처된 상태 객체를 받아와서 현재 상태를 복원
    public void RestoreState(object state)
    {
        var data = state as EventSaveData;
        if (data == null)
        {
            data = new EventSaveData();
            data.LastEventID = null;
        }
        Init(data.LastEventID);
    }
}

