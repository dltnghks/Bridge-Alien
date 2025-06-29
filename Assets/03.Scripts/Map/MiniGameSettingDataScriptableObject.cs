using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameSettingData", menuName = "Game/Data/MiniGameSettingData")]
public class MiniGameSettingDataScriptableObject : ScriptableObject
{
    // JSON 문자열을 저장하는 딕셔너리
    public SerializedDictionary<Define.MiniGameType, string> miniGameSettingData = new SerializedDictionary<Define.MiniGameType, string>();

    /// <summary>
    /// JSON 데이터를 받아서 Dictionary에 저장
    /// </summary>
    public void SetData(string jsonText)
    {
        try
        {
            // JSON을 Dictionary<string, object> 형태로 저장
            Dictionary<string, object> parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);

            foreach (var key in parsedData.Keys)
            {
                if (System.Enum.TryParse(key, out Define.MiniGameType miniGameType))
                {
                    // JSON 객체를 문자열로 변환 후 저장
                    string jsonString = JsonConvert.SerializeObject(parsedData[key]);
                    miniGameSettingData[miniGameType] = jsonString;
                }
                else
                {
                    Debug.LogWarning($"⚠️ {key}를 Define.MiniGameType으로 변환할 수 없습니다.");
                }
            }

            Debug.Log($"✅ MiniGameSetting Data Loaded: {miniGameSettingData.Count} types loaded.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 데이터 파싱 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// 저장된 JSON을 특정 클래스로 변환하여 반환하는 함수
    /// </summary>
    public T GetMiniGameSettings<T>(Define.MiniGameType miniGameType) where T : MiniGameSettingBase
    {
        if (miniGameSettingData.ContainsKey(miniGameType))
        {
            try
            {
                string jsonData = miniGameSettingData[miniGameType];
                Debug.Log($"📤 GetMiniGameSettings - {miniGameType}: {jsonData}");

                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ {miniGameType} 데이터 변환 중 오류 발생: {e.Message}");
            }
        }

        Debug.LogWarning($"⚠️ {miniGameType} 데이터가 존재하지 않습니다.");
        return null;
    }
}
