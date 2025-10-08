using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[System.Serializable]
public class EventData
{
    public string EventID;
    public Define.EventType EventType;
    public string Parameter;                    //다이얼로그면 다이얼로그 아이디, 게임이면 게임 타입..
    public string NextEventID;
    public Define.DialogSceneType DialogScene;

    public T GetParameter<T>()
    {
        return Utils.ParseEnum<T>(Parameter);
    }
    
}

