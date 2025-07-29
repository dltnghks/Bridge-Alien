using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[System.Serializable]
public class DailyData
{
    public string EventID;
    public int Time;
    public Define.DailyEventType EventType;
    public string Parameter;
    public string NextEventID;
    public Define.DialogSceneType DialogScene;

    public T GetParameter<T>()
    {
        return Utils.ParseEnum<T>(Parameter);
    }
    
}

