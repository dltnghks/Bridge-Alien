using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public class DataManager : MonoBehaviour
{
    private const string _dialogDataURL = "https://script.google.com/macros/s/AKfycbzuvwBUm_3OQSiRMeq8sH3B9bwk1zHblthyrvoRK4JqZeKlNqd2mgh-PO0OEpqriTb_/exec"; // Google Apps Script에서 생성한 URL
    
    // 각 데이터를 관리하는 매니저
    public DialogDataManager DialogDataManager;
    
    // 데이터 로드 완료 이벤트 (필요하면 UI 업데이트 등과 연결 가능)
    public event Action<Define.DataType> OnDataLoaded;

    private int totalLoadCount = 3; // 로드해야 할 데이터 개수
    private int loadedCount = 0;

    public event Action OnAllDataLoaded; // 모든 데이터가 로드되었을 때 발생하는 이벤트

    public void Init()
    {
        DialogDataManager = new DialogDataManager();
        
        // 필요한 데이터 매니저 추가


        LoadAllData();
    }
    
    /// <summary>
    /// 모든 데이터를 로드하는 함수 (게임 시작 시 실행)
    /// </summary>
    public void LoadAllData()
    {
        StartCoroutine(LoadDataRoutine("Dialog", _dialogDataURL, (json) =>
        {
            DialogDataManager.SetData(json);
        }));
        
        // 추가적으로 필요한 데이터 로드
    }

    /// <summary>
    /// Google Sheets에서 데이터를 가져와서 특정 매니저에 전달하는 코루틴
    /// </summary>
    private IEnumerator LoadDataRoutine(string dataType, string url, Action<string> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonText = request.downloadHandler.text;
                Debug.Log($"📥 [{dataType}] JSON Data Received:\n{jsonText}");

                onSuccess?.Invoke(jsonText);
                loadedCount++;

                // 모든 데이터가 로드되었으면 이벤트 호출
                if (loadedCount >= totalLoadCount)
                {
                    Debug.Log("✅ 모든 데이터 로드 완료!");
                    OnAllDataLoaded?.Invoke();
                }
            }
            else
            {
                Debug.LogError($"❌ [{dataType}] 데이터 로드 실패: {request.error}");
            }
        }
    }
}