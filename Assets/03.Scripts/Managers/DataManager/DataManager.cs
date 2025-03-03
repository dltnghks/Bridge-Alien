using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using UnityEngine.Serialization;

public class DataManager : MonoBehaviour
{
    // Google Apps Script에서 생성한 URL
    private const string _dialogDataURL = "https://script.google.com/macros/s/AKfycbzuvwBUm_3OQSiRMeq8sH3B9bwk1zHblthyrvoRK4JqZeKlNqd2mgh-PO0OEpqriTb_/exec"; 
    private const string _miniGameSettingDataURL = "https://script.google.com/macros/s/AKfycbyCzaXRCmG8TwN7bjGK23w-YysJzMeB6_SBvJ_zDz4j8h1FmPJmw51V-x0FqMFpt-NI/exec";
    private const string _dailyDataURL = "https://script.google.com/macros/s/AKfycby0iTsBcBmQ6zzWPfbS0FQylx_7txq6JyrB65AlJgp7aClgChXycrZrodxg_-tezb9Baw/exec";
    
    // 각 데이터를 관리하는 매니저
    public DialogDataManager DialogDataManager;
    public MiniGameSettingDataManager MiniGameSettingDataManager;
    public DailyDataManager DailyDataManager;
    
    // 데이터 로드 완료 이벤트 (필요하면 UI 업데이트 등과 연결 가능)
    public event Action<Define.DataType> OnDataLoaded;

    public int TotalLoadCount = (int)Define.DataType.End; // 로드해야 할 데이터 개수
    public int LoadedCount = 0;
    public bool IsLoading = false;

    public event Action OnAllDataLoaded; // 모든 데이터가 로드되었을 때 발생하는 이벤트

    public void Init()
    {   
        DialogDataManager = new DialogDataManager();
        MiniGameSettingDataManager = new MiniGameSettingDataManager();
        DailyDataManager = new DailyDataManager();
    }
    
    
    /// 모든 데이터를 로드하는 함수 (게임 시작 시 실행)
    public void LoadAllData()
    {
        // 데이터를 로드하지 않았을 때만 로드하기
        if (IsLoading == false)
        {
            StartCoroutine(LoadDataRoutine(Define.DataType.Dialog, _dialogDataURL,
                (json) => { DialogDataManager.SetData(json); }));

            StartCoroutine(LoadDataRoutine(Define.DataType.MiniGameSetting, _miniGameSettingDataURL,
                (json) => { MiniGameSettingDataManager.SetData(json); }));

            
            StartCoroutine(LoadDataRoutine(Define.DataType.Daily, _dailyDataURL,
                (json) => { DailyDataManager.SetData(json); }));


            
            // 추가적으로 필요한 데이터 로드
            
            IsLoading = true;
        }
    }

    /// Google Sheets에서 데이터를 가져와서 특정 매니저에 전달하는 코루틴
    private IEnumerator LoadDataRoutine(Define.DataType dataType, string url, Action<string> onSuccess)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {   
            yield return request.SendWebRequest();
            
            Logger.Log("로드 시작");
            OnDataLoaded?.Invoke(dataType);
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonText = request.downloadHandler.text;
                Logger.Log($"📥 [{dataType.ToString()}] JSON Data Received:\n{jsonText}");

                onSuccess?.Invoke(jsonText);
                LoadedCount++;

                // 모든 데이터가 로드되었으면 이벤트 호출
                if (LoadedCount >= TotalLoadCount)
                {
                    Logger.Log("✅ 모든 데이터 로드 완료!");
                    OnAllDataLoaded?.Invoke();
                }
            }
            else
            {
                Logger.LogError($"❌ [{dataType}] 데이터 로드 실패: {request.error}");
            }
        }
    }
}