using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

public class GoogleSheetsImporter : EditorWindow
{
    // Google Apps Script에서 생성한 URL
    private string _dialogDataURL = "https://script.google.com/macros/s/AKfycbwgAd7p7krlRhlz0xeJHxjA8BhQ5hUEkTmdIrOGZNqPHXyvAPIkZCC-yAJ9PRaaqPU8/exec"; 
    private string _miniGameSettingDataURL = "https://script.google.com/macros/s/AKfycbyCzaXRCmG8TwN7bjGK23w-YysJzMeB6_SBvJ_zDz4j8h1FmPJmw51V-x0FqMFpt-NI/exec";
    private string _dailyDataURL = "https://script.google.com/macros/s/AKfycbztQL4Jix2cBBlocYbbYOuUuPCbU-ExjG1q1cgoXlAwCL2dsRK_vyBnk_uiEBXyPPbNgw/exec";
    private string _playerTaskDataURL = "https://script.google.com/macros/s/AKfycbycs1iOHWpQoMCrt-OWwXdaH0sqgoG5GzvRagF0cBDqO4U0MEoRiRlx-q_SV7uWFSl1/exec";
    private string savePath = "Assets/Resources";

    [MenuItem("Tools/GoogleSheetsImporter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(GoogleSheetsImporter));
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheets Importer", EditorStyles.boldLabel);
        
        savePath = EditorGUILayout.TextField("Save Path", savePath);
        
        _dialogDataURL = EditorGUILayout.TextField("DialogData Sheet URL", _dialogDataURL);
        if (GUILayout.Button("Import Dialog Data"))
            ImportData(_dialogDataURL, Define.DataType.Dialog);
        
        _miniGameSettingDataURL = EditorGUILayout.TextField("MiniGameSettingData Sheet URL", _miniGameSettingDataURL);
        if (GUILayout.Button("Import MiniGameSetting Data"))
            ImportData(_miniGameSettingDataURL, Define.DataType.MiniGameSetting);
            
        _dailyDataURL = EditorGUILayout.TextField("DailyData Sheet URL", _dailyDataURL);
        if (GUILayout.Button("Import Daily Data"))
            ImportData(_dailyDataURL, Define.DataType.Daily);
            
        _playerTaskDataURL = EditorGUILayout.TextField("PlayerTaskData Sheet URL", _playerTaskDataURL);
        if (GUILayout.Button("Import PlayerTask Data"))
            ImportData(_playerTaskDataURL, Define.DataType.PlayerTask);

        if (GUILayout.Button("Import All Data"))
        {
            ImportData(_dialogDataURL, Define.DataType.Dialog);
            ImportData(_miniGameSettingDataURL, Define.DataType.MiniGameSetting);
            ImportData(_dailyDataURL, Define.DataType.Daily);
            ImportData(_playerTaskDataURL, Define.DataType.PlayerTask);
        }
    }

    private void ImportData(string url, Define.DataType dataType)
    {
        try
        {
            WebClient client = new WebClient();
            string jsonData = client.DownloadString(url);

            if (!string.IsNullOrEmpty(jsonData))
            {
                CreateOrUpdateScriptableObject(jsonData, dataType);
            }
            else
            {
                Debug.LogError($"❌ {dataType} 데이터를 다운로드했지만 JSON이 비어 있음.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ {dataType} 데이터를 다운로드하는 중 오류 발생: {ex.Message}");
        }
    }

    private void CreateOrUpdateScriptableObject(string jsonData, Define.DataType dataType)
    {
        string assetPath = $"{savePath}/{dataType}Data.asset";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);
        
        switch (dataType)
        {
            case Define.DataType.Daily:
                DailyDataScriptableObject dailyData = CreateInstance<DailyDataScriptableObject>();
                dailyData.SetData(jsonData);
                AssetDatabase.CreateAsset(dailyData, assetPath);
                break;
            case Define.DataType.Dialog:
                DialogDataScriptableObject dialogData = CreateInstance<DialogDataScriptableObject>();
                dialogData.SetData(jsonData);
                AssetDatabase.CreateAsset(dialogData, assetPath);
                break;
            case Define.DataType.MiniGameSetting:
                MiniGameSettingDataScriptableObject miniGameData = CreateInstance<MiniGameSettingDataScriptableObject>();
                miniGameData.SetData(jsonData);
                AssetDatabase.CreateAsset(miniGameData, assetPath);
                break;
            case Define.DataType.PlayerTask:
                PlayerTaskDataScriptableObject playerTaskData = CreateInstance<PlayerTaskDataScriptableObject>();
                playerTaskData.SetData(jsonData);
                AssetDatabase.CreateAsset(playerTaskData, assetPath);
                break;
        }

        Debug.Log($"✅ {dataType} ScriptableObject 생성 완료: {assetPath}");

        AssetDatabase.SaveAssets();
    }
}
