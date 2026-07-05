using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private Define.EventDataID _lastEventDataID;
    private EventData _curEventData;
    private Dictionary<string, EventData> _currentEventDataDict = new Dictionary<string, EventData>();
    public EventData CurEventData => _curEventData;
    private UIDialogPopup _dialogPopup = null;
    private bool _isStageStoryMode;
    private Define.Scene _nextSceneOnEnd = Define.Scene.House;

    public void Init(Define.EventDataID lastEventDataID = Define.EventDataID.Unknown, Define.Scene nextSceneOnEnd = Define.Scene.House)
    {
        Logger.Log("Event Manager Init");
        _isStageStoryMode = false;
        _nextSceneOnEnd = nextSceneOnEnd;
        _lastEventDataID = lastEventDataID;
        if (_lastEventDataID == Define.EventDataID.Unknown)
        {
            Logger.Log("EventDataID is Unknown");
            return;   
        }
        SetEventData();
    }

    public void InitStageStory(Define.EventDataID eventDataID)
    {
        Logger.Log("Stage Story Event Manager Init");
        _isStageStoryMode = true;
        _nextSceneOnEnd = Define.Scene.House;
        _lastEventDataID = eventDataID;
        if (_lastEventDataID == Define.EventDataID.Unknown)
        {
            Logger.Log("EventDataID is Unknown");
            return;
        }

        SetEventData();
    }

    private void SetEventData(bool playImmediately = true)
    {
        Logger.Log($"Set Event Data : {_lastEventDataID}");
        // DataManager에서 curDate 세팅 값 가져오기
        _currentEventDataDict = Managers.Data.EventData.GetData(_lastEventDataID);

        if (_currentEventDataDict == null)
        {
            Logger.LogError("CurrentEventDataDict is null");
            return;
        }

        // 시작 이벤트로 설정
        _curEventData = _currentEventDataDict["Start"];
        if (_curEventData == null)
        {
            Logger.LogError("Start Event is null");
            return;
        }
        PlayEvent();
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
        if (_curEventData == null)
        {
            Logger.LogError("Current Event Data is null");
            return;
        }


        if (_curEventData.EventType == Define.EventType.Unknown)
        {
            Managers.Scene.ChangeScene(Define.Scene.EventScene);
        }
        // check event type
        else if (_curEventData.EventType == Define.EventType.Dialog)
        {
            PlayDialog();
        }
        else if (_curEventData.EventType == Define.EventType.MiniGame)
        {
            if (_isStageStoryMode)
            {
                EndEvent();
                return;
            }

            // 게임을 클리어했을 때만 다음 이벤트 진행
            PlayGame();
            return;
        }
        else if (_curEventData.EventType == Define.EventType.End)
        {
            EndEvent();
            return;
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
        _isStageStoryMode = false;
        _dialogPopup = null;
        Define.Scene nextScene = _nextSceneOnEnd;
        _nextSceneOnEnd = Define.Scene.House;
        Managers.Scene.ChangeScene(nextScene);
    }

    private void EndStageStory()
    {
        _isStageStoryMode = false;
        _dialogPopup = null;
        Managers.Scene.ChangeScene(Define.Scene.House);
    }
}

