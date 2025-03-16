using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class DailyDataManager
{
    private Dictionary<string, List<DailyData>> dailyData = new Dictionary<string, List<DailyData>>();

    /// <summary>
    /// DataManager에서 JSON을 받아와 데이터를 설정하는 함수
    /// </summary>
    public void SetData(string jsonText)
    {
        Dictionary<string, List<DailyData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<DailyData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            dailyData[key] = parsedData[key];
        }

        Debug.Log($"✅ Dialog Data Loaded: {dailyData.Count} types loaded.");
    }

    public Dictionary<string, DailyData> GetData(string date)
    {
        if (dailyData.ContainsKey(date))
        {
            Dictionary<string, DailyData> returnData = new Dictionary<string, DailyData>();

            foreach (DailyData data in dailyData[date])
            {
                returnData[data.EventID] = data;
            }
            
            return returnData;
        }

        Debug.LogWarning($"⚠️ Daily Data {date} is null");
        return null;
    }
}
