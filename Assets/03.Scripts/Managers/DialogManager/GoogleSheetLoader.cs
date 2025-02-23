using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

public class GoogleSheetLoader : MonoBehaviour
{
    private string jsonUrl = "https://script.google.com/macros/s/AKfycbz3l-v31HSuxUYiaqUjUg-b6vGRT_O1RYSRpY0-H5t2uNyDg5MMsVhwrtJOqs6bYlEb/exec"; // Google Apps Script에서 생성한 URL

    private Dictionary<string, List<DialogueEntry>> allDialogues = new Dictionary<string, List<DialogueEntry>>();

    public void LoadAllSheets()
    {
        StartCoroutine(GetJsonData());
    }

    private IEnumerator GetJsonData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonText = request.downloadHandler.text;
//                Debug.Log("📥 JSON Data Received:\n" + jsonText);

                allDialogues = JsonConvert.DeserializeObject<Dictionary<string, List<DialogueEntry>>>(jsonText);
                PrintAllDialogues();
            }
            else
            {
                Debug.LogError($"❌ JSON 데이터 로드 실패: {request.error}");
            }
        }
    }

    /// <summary>
    /// 모든 시트의 대사를 출력하는 함수
    /// </summary>
    public void PrintAllDialogues()
    {
        foreach (var sheet in allDialogues)
        {
            Debug.Log($"=== [{sheet.Key}] 시트 데이터 ===");
            foreach (var entry in sheet.Value)
            {
                Debug.Log($"{entry.character}: {entry.dialogue}");
            }
        }
    }
}

[System.Serializable]
public class DialogueEntry
{
    public string character;
    public string dialogue;
}