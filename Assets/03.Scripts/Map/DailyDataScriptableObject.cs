using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "DailyData", menuName = "Game/Data/DailyData")]
public class DailyDataScriptableObject : ScriptableObject
{
    public SerializedDictionary<string, List<DailyData>> DailyData = new SerializedDictionary<string, List<DailyData>>();

    public void SetData(string jsonText)
    {
        Dictionary<string, List<DailyData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<DailyData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            DailyData[key] = parsedData[key];
        }

        Debug.Log($"✅ Dialog Data Loaded: {DailyData.Count} types loaded.");
    }
    
    public Dictionary<string, DailyData> GetData(string date)
    {
        if (DailyData.ContainsKey(date))
        {
            Dictionary<string, DailyData> returnData = new Dictionary<string, DailyData>();

            foreach (DailyData data in DailyData[date])
            {
                returnData[data.EventID] = data;
            }
            
            return returnData;
        }

        Debug.LogWarning($"⚠️ Daily Data {date} is null");
        return null;
    }
}
