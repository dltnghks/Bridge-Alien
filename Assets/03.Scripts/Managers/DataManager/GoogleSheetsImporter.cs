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
    private string savePath = "Assets/Resources";

    [MenuItem("Tools/GoogleSheetsImporter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(GoogleSheetsImporter));
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheets Importer", EditorStyles.boldLabel);
        _dialogDataURL = EditorGUILayout.TextField("DialogData Sheet URL", _dialogDataURL);
        _miniGameSettingDataURL = EditorGUILayout.TextField("MiniGameSettingData Sheet URL", _miniGameSettingDataURL);
        _dailyDataURL = EditorGUILayout.TextField("DailyData Sheet URL", _dailyDataURL);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Import Data"))
        {
            ImportData(_dialogDataURL, Define.DataType.Dialog);
            ImportData(_miniGameSettingDataURL, Define.DataType.MiniGameSetting);
            ImportData(_dailyDataURL, Define.DataType.Daily);
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
        }

        Debug.Log($"✅ {dataType} ScriptableObject 생성 완료: {assetPath}");

        AssetDatabase.SaveAssets();
    }
}
