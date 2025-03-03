using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class MiniGameSettingDataManager
{
    private Dictionary<Define.MiniGameType, object> miniGameSettingData = new Dictionary<Define.MiniGameType, object>();

    /// DataManager에서 JSON을 받아와 데이터를 설정하는 함수
    public void SetData(string jsonText)
    {
        try
        {
            // JSON을 Dictionary<string, object> 형태로 파싱
            Dictionary<string, object> parsedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonText);

            foreach (var key in parsedData.Keys)
            {
                if (System.Enum.TryParse(key, out Define.MiniGameType miniGameType))
                {
                    // 원본 데이터를 그대로 저장
                    miniGameSettingData[miniGameType] = parsedData[key];
                }
                else
                {
                    Logger.LogWarning($"{key}를 Define.MiniGameType으로 변환할 수 없습니다.");
                }
            }

            Logger.Log($"MiniGameSetting Data Loaded: {miniGameSettingData.Count} types loaded.");
        }
        catch (System.Exception e)
        {
            Logger.LogError($"데이터 파싱 중 오류 발생: {e.Message}");
        }
    }

    /// 저장된 데이터를 특정 클래스로 변환하여 반환하는 함수
    public T GetMiniGameSettings<T>(Define.MiniGameType miniGameType) where T : class
    {
        if (miniGameSettingData.ContainsKey(miniGameType))
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(miniGameSettingData[miniGameType], Formatting.Indented);
                Logger.Log($"📤 GetMiniGameSettings - {miniGameType}: {jsonData}");

                return JsonConvert.DeserializeObject<T>(jsonData); // 객체 변환
            }
            catch (System.Exception e)
            {
                Logger.LogError($"❌ {miniGameType} 데이터 변환 중 오류 발생: {e.Message}");
            }
        }
        return null;
    }

}
