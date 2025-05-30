using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Game/Data/DialogData")]
public class DialogDataScriptableObject : ScriptableObject
{
    public SerializedDictionary<Define.Dialog, List<DialogData>> DialogData = new SerializedDictionary<Define.Dialog, List<DialogData>>();

    /// <summary>
    /// DataManager에서 JSON을 받아와 데이터를 설정하는 함수
    /// </summary>
    public void SetData(string jsonText)
    {
        Dictionary<string, List<DialogData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<DialogData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            if (System.Enum.TryParse(key, out Define.Dialog dialogType))
            {
                DialogData[dialogType] = parsedData[key];
            }
            else
            {
                Debug.LogWarning($"⚠️ {key}를 Define.DialogType으로 변환할 수 없습니다.");
            }
        }

        Debug.Log($"✅ Dialog Data Loaded: {DialogData.Count} types loaded.");
    }

    public List<DialogData> GetData(Define.Dialog dialog)
    {
        if (DialogData.ContainsKey(dialog))
        {
            return DialogData[dialog];
        }

        Debug.LogWarning($"⚠️ Dialog Data {dialog} is null");
        return null;
    }
}