using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class DialogDataManager
{
    private Dictionary<Define.DialogType, List<DialogData>> dialogData = new Dictionary<Define.DialogType, List<DialogData>>();

    /// <summary>
    /// DataManager에서 JSON을 받아와 데이터를 설정하는 함수
    /// </summary>
    public void SetData(string jsonText)
    {
        Dictionary<string, List<DialogData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<DialogData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            if (System.Enum.TryParse(key, out Define.DialogType dialogType))
            {
                dialogData[dialogType] = parsedData[key];
            }
            else
            {
                Debug.LogWarning($"⚠️ {key}를 Define.DialogType으로 변환할 수 없습니다.");
            }
        }

        Debug.Log($"✅ Dialog Data Loaded: {dialogData.Count} types loaded.");
    }

    public List<DialogData> GetData(Define.DialogType dialogType)
    {
        if (dialogData.ContainsKey(dialogType))
        {
            return dialogData[dialogType];
        }

        Debug.LogWarning($"⚠️ Dialog Data {dialogType} is null");
        return null;
    }
}