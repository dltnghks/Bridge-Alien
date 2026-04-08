using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Game/Data/DialogData")]
public class DialogDataScriptableObject : ScriptableObject
{
    [Serializable]
    private class DialogBGMRow
    {
        public string DialogKey;
        public string BGMID;
    }

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
        JObject root = JsonConvert.DeserializeObject<JObject>(jsonText);
        DialogData.Clear();
        DialogBGM.Clear();

        if (root == null)
        {
            Debug.LogError("Dialog JSON is empty or invalid.");
            InitDialogBGMDict();
            return;
        }

        foreach (JProperty property in root.Properties())
        {
            if (property.Name == nameof(DialogBGM))
            {
                Debug.Log($"DialogBGM JSON found: {property.Value}");
                ParseDialogBGM(property.Value);
                continue;
            }

            if (Enum.TryParse(property.Name, true, out Define.Dialog dialogType))
            {
                List<DialogData> dialogList = property.Value.ToObject<List<DialogData>>();
                DialogData[dialogType] = dialogList ?? new List<DialogData>();
            }
            else
            {
                Debug.LogWarning($"Dialog key {property.Name} could not be converted to Define.Dialog.");
            }
        }

        InitDialogBGMDict();
        Debug.Log($"Dialog Data Loaded: {DialogData.Count} types loaded. DialogBGM count: {DialogBGM.Count}");
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

    private void ParseDialogBGM(JToken token)
    {
        List<DialogBGMRow> bgmRows = token.ToObject<List<DialogBGMRow>>();
        if (bgmRows == null)
        {
            Debug.LogWarning("DialogBGM token could not be parsed as a list.");
            return;
        }

        Debug.Log($"DialogBGM rows parsed: {bgmRows.Count}");

        foreach (DialogBGMRow row in bgmRows)
        {
            if (string.IsNullOrWhiteSpace(row.DialogKey))
            {
                Debug.LogWarning("DialogBGM row skipped because DialogKey is empty.");
                continue;
            }

            string dialogKeyText = row.DialogKey.Trim();
            string bgmIdText = row.BGMID?.Trim();

            if (Enum.TryParse(dialogKeyText, true, out Define.Dialog dialogKey) == false)
            {
                Debug.LogWarning($"DialogBGM key {row.DialogKey} could not be converted to Define.Dialog.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(bgmIdText) ||
                Enum.TryParse(bgmIdText, true, out global::DialogBGM bgmValue) == false)
            {
                Debug.LogWarning($"DialogBGM value {row.BGMID} could not be converted to DialogBGM.");
                continue;
            }

            DialogBGM[dialogKey] = bgmValue;
            Debug.Log($"DialogBGM mapped: {dialogKey} -> {bgmValue}");
        }
    }
}
