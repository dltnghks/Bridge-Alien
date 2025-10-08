using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "Game/Data/EventData")]
public class EventDataScriptableObject : ScriptableObject
{
    public SerializedDictionary<string, List<EventData>> EventData = new SerializedDictionary<string, List<EventData>>();

    public void SetData(string jsonText)
    {
        Dictionary<string, List<EventData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<EventData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            EventData[key] = parsedData[key];
        }

        Debug.Log($"✅ Dialog Data Loaded: {EventData.Count} types loaded.");
    }
    
    public Dictionary<string, EventData> GetData(string date)
    {
        if (EventData.ContainsKey(date))
        {
            Dictionary<string, EventData> returnData = new Dictionary<string, EventData>();

            foreach (EventData data in EventData[date])
            {
                returnData[data.EventID] = data;
            }
            
            return returnData;
        }

        Debug.LogWarning($"⚠️ Event Data {date} is null");
        return null;
    }
}
