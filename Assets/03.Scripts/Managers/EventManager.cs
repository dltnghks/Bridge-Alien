using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    private EventData _curEventData;

    public EventData CurEventData => _curEventData;

    public void SetEventData(EventData eventData)
    {
        _curEventData = eventData;
    }

    public void PlayEvent()
    {
        // check event type
        if (_curEventData.EventType == Define.EventType.Dialog)
        {

        }
        else if (_curEventData.EventType == Define.EventType.End)
        {
            EndEvent();
        }
        else
        {
            // type set error
            Logger.LogError("Error : Current event type is not define");
            EndEvent();
        }
    }

    public void EndEvent()
    {
        
    }
}

