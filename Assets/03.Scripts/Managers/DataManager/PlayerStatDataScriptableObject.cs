using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Game/Data/PlayerStatData")]
public class PlayerStatDataScriptableObject : ScriptableObject
{
    public PlayerStatData PlayerStatData = new PlayerStatData();
    
    public void SetData(string jsonText)
    {
        try
        {
            PlayerStatData parsedData = JsonConvert.DeserializeObject<PlayerStatData>(jsonText);

            if (parsedData != null)
            {
                PlayerStatData = parsedData;   
            }
            else
            {
                Debug.LogWarning("Not able to parse player stat data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 데이터 파싱 중 오류 발생: {e.Message}");
        }
    }
}
