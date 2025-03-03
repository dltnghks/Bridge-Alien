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
            dailyData["default"] = parsedData[key];
        }

        Debug.Log($"✅ Dialog Data Loaded: {dailyData.Count} types loaded.");
    }

    public DailyData GetData(int date, string dailyDataType = "default")
    {
        if (dailyData.ContainsKey(dailyDataType) && dailyData[dailyDataType].Count > date)
        {
            return dailyData[dailyDataType][date];
        }

        Debug.LogWarning($"⚠️ Dialog Data {dailyDataType} is null");
        return null;
    }
}
