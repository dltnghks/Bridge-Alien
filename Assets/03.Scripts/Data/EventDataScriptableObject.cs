using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Game/Data/EventData")]
public class EventDataScriptableObject : ScriptableObject
{
    public SerializedDictionary<Define.EventDataID, List<EventData>> EventData = new SerializedDictionary<Define.EventDataID, List<EventData>>();

    public void SetData(string jsonText)
    {
        Dictionary<Define.EventDataID, List<EventData>> parsedData = JsonConvert.DeserializeObject<Dictionary<Define.EventDataID, List<EventData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            EventData[key] = parsedData[key];
        }

        Debug.Log($"✅ Dialog Data Loaded: {EventData.Count} types loaded.");
    }
    
    public Dictionary<string, EventData> GetData(Define.EventDataID eventDataID)
    {
        if (EventData.ContainsKey(eventDataID))
        {
            Dictionary<string, EventData> returnData = new Dictionary<string, EventData>();

            foreach (EventData data in EventData[eventDataID])
            {
                returnData[data.EventID] = data;
            }
            
            return returnData;
        }

        Debug.LogWarning($"⚠️ Event Data {eventDataID} is null");
        return null;
    }
}
