using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class DataManager : MonoBehaviour
{
    private const string _dialogDataURL = "https://script.google.com/macros/s/AKfycbz2sdTml4-n-bPLZVbsTGpg78Y0U_1fESlHY4VlSfOBFlv7-uJmrnnsu2S5kVn2UnWx/exec"; // Google Apps Script에서 생성한 URL
    
    // 다양한 데이터를 저장할 Dictionary
    private Dictionary<Define.DataType, object> _dataStorage = new Dictionary<Define.DataType, object>();

    // 데이터 로드 완료 이벤트 (필요하면 UI 업데이트 등과 연결 가능)
    public event Action<Define.DataType> OnDataLoaded;

    public void Init()
    {
        LoadData<DialogueData>(Define.DataType.Dialog, _dialogDataURL);
        
        // 추가적으로 Load할 데이터 작성, DataType 추가와 URL 기록
        
    }

    /// <summary>
    /// Google Sheets에서 데이터를 불러오는 함수 (key: 데이터 키, url: Google Sheets JSON URL)
    /// </summary>
    public void LoadData<T>(Define.DataType key, string url)
    {
        StartCoroutine(GetJsonData<T>(key, url));
    }

    private IEnumerator GetJsonData<T>(Define.DataType key, string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonText = request.downloadHandler.text;
                Logger.Log($"📥 [{key}] JSON Data Received:\n" + jsonText);

                // JSON을 제네릭 타입(T)으로 변환
                T parsedData = JsonConvert.DeserializeObject<T>(jsonText);

                // 데이터 저장
                if (!_dataStorage.ContainsKey(key))
                {
                    _dataStorage[key] = parsedData;
                }
                else
                {
                    _dataStorage[key] = parsedData; // 기존 데이터 덮어쓰기
                }

                // 데이터 로드 완료 이벤트 발생
                OnDataLoaded?.Invoke(key);
            }
            else
            {
                Logger.LogError($"❌ [{key}] 데이터 로드 실패: {request.error}");
            }
        }
    }

    /// <summary>
    /// 저장된 데이터를 특정 타입(T)으로 가져오는 함수
    /// </summary>
    public T GetData<T>(Define.DataType key)
    {
        if (_dataStorage.ContainsKey(key))
        {
            return (T)_dataStorage[key];
        }

        Logger.LogWarning($"⚠️ {key} 데이터가 존재하지 않습니다.");
        return default;
    }

    /// <summary>
    /// 모든 데이터를 출력하는 함수 (디버깅용)
    /// </summary>
    public void PrintAllData()
    {
        foreach (var key in _dataStorage.Keys)
        {
            Logger.Log($"=== [{key}] 데이터 ===\n{JsonConvert.SerializeObject(_dataStorage[key], Formatting.Indented)}");
        }
    }
}