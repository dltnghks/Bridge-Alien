using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTaskData", menuName = "Game/Data/PlayerTaskData")]
public class PlayerTaskDataScriptableObject : ScriptableObject
{
    // JSON 문자열을 저장하는 딕셔너리
    public SerializedDictionary<Define.TaskType, List<PlayerTaskData>> PlayerTaskData = new SerializedDictionary<Define.TaskType, List<PlayerTaskData>>();

    /// <summary>
    /// DataManager에서 JSON을 받아와 데이터를 설정하는 함수
    /// </summary>
    public void SetData(string jsonText)
    {
        Dictionary<string, List<PlayerTaskData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<PlayerTaskData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            if (System.Enum.TryParse(key, out Define.TaskType type))
            {
                PlayerTaskData[type] = parsedData[key];
            }
            else
            {
                Debug.LogWarning($"⚠️ {key}를 Define.DialogType으로 변환할 수 없습니다.");
            }
        }

        Debug.Log($"✅ Dialog Data Loaded: {PlayerTaskData.Count} types loaded.");
    }

    public List<PlayerTaskData> GetData(Define.TaskType taskType)
    {
        if (PlayerTaskData.ContainsKey(taskType))
        {
            return PlayerTaskData[taskType];
        }
        
        Logger.LogError("Not Found PlayerTaskData");
        return null;
    }

}
