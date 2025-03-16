using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Serialization;

[System.Serializable]
public class DailyData
{
    public string EventID { get; set; }
    public int Time { get; set; }
    public Define.DailyEventType EventType { get; set; }
    public string Parameter { get; set; }
    public string NextEventID { get; set; }

    public T GetParameter<T>()
    {
        return Utils.ParseEnum<T>(Parameter);
    }
    
}

