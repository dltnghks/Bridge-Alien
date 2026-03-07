using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Game/Data/DialogData")]
public class DialogDataScriptableObject : ScriptableObject
{
    public SerializedDictionary<Define.Dialog, List<DialogData>> DialogData = new SerializedDictionary<Define.Dialog, List<DialogData>>();
    public SerializedDictionary<Define.Dialog, DialogBGM> DialogBGM = new SerializedDictionary<Define.Dialog, DialogBGM>();

    public void OnEnable()
    {
        InitDialogBGMDict();
    }

    /// <summary>
    /// DataManager에서 JSON을 받아와 데이터를 설정하는 함수
    /// </summary>
    public void SetData(string jsonText)
    {
        Dictionary<string, List<DialogData>> parsedData = JsonConvert.DeserializeObject<Dictionary<string, List<DialogData>>>(jsonText);

        foreach (var key in parsedData.Keys)
        {
            if (Enum.TryParse(key, out Define.Dialog dialogType))
            {
                DialogData[dialogType] = parsedData[key];
            }
            else
            {
                Debug.LogWarning($"Dialog key {key} could not be converted to Define.Dialog.");
            }
        }

        InitDialogBGMDict();
        Debug.Log($"Dialog Data Loaded: {DialogData.Count} types loaded.");
    }

    public List<DialogData> GetData(Define.Dialog dialog)
    {
        if (DialogData.ContainsKey(dialog))
        {
            return DialogData[dialog];
        }

        Debug.LogWarning($"Dialog Data {dialog} is null");
        return null;
    }

    public DialogBGM GetBGM(Define.Dialog dialog)
    {
        InitDialogBGMDict();

        if (DialogBGM.TryGetValue(dialog, out DialogBGM bgmKey))
        {
            return bgmKey;
        }

        return global::DialogBGM.Default;
    }

    private void InitDialogBGMDict()
    {
        foreach (Define.Dialog dialog in Enum.GetValues(typeof(Define.Dialog)))
        {
            if (dialog == Define.Dialog.Unknown)
            {
                continue;
            }

            if (DialogBGM.ContainsKey(dialog) == false)
            {
                DialogBGM.Add(dialog, global::DialogBGM.Default);
            }
        }
    }
}
