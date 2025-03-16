using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using UnityEngine.Serialization;

public class DataManager : MonoBehaviour
{
    // 각 데이터를 관리하는 매니저
    public DialogDataScriptableObject DialogData;
    public MiniGameSettingDataScriptableObject MiniGameData;
    public DailyDataScriptableObject DailyData;
    
    // 데이터 로드 완료 이벤트 (필요하면 UI 업데이트 등과 연결 가능)
    public event Action<Define.DataType> OnDataLoaded;

    public int TotalLoadCount = 0; // (int)Define.DataType.End-1; // 로드해야 할 데이터 개수
    public int LoadedCount = 0;
    public bool IsLoading = false;

    public event Action OnAllDataLoaded; // 모든 데이터가 로드되었을 때 발생하는 이벤트

    public void Init()
    {
        LoadAllData();
    }
    
    
    /// 모든 데이터를 로드하는 함수 (게임 시작 시 실행)
    public void LoadAllData()
    {
        if (IsLoading == false)
        {
            string dialogDataPath = Define.DataType.Dialog.ToString() + "Data";
            string miniGameDataPath = Define.DataType.MiniGameSetting.ToString() + "Data";
            string dailyDataPath = Define.DataType.Daily.ToString() + "Data";

            Debug.Log($"🔍 Loading DialogData from: {dialogDataPath}");
            DialogData = Resources.Load<DialogDataScriptableObject>(dialogDataPath);

            Debug.Log($"🔍 Loading MiniGameSettingData from: {miniGameDataPath}");
            MiniGameData = Resources.Load<MiniGameSettingDataScriptableObject>(miniGameDataPath);

            Debug.Log($"🔍 Loading DailyData from: {dailyDataPath}");
            DailyData = Resources.Load<DailyDataScriptableObject>(dailyDataPath);

            // 데이터가 정상적으로 로드되었는지 확인
            if (DialogData == null) Debug.LogError("❌ DialogDataScriptableObject not found!");
            if (MiniGameData == null) Debug.LogError("❌ MiniGameSettingDataScriptableObject not found!");
            if (DailyData == null) Debug.LogError("❌ DailyDataScriptableObject not found!");

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