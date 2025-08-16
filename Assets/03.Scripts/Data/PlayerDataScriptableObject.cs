using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Data/PlayerStatData")]
public class PlayerDataScriptableObject : ScriptableObject
{
    public PlayerData playerData = new PlayerData();
    
    public void SetData(string jsonText)
    {
        try
        {
            PlayerData parsedData = JsonConvert.DeserializeObject<PlayerData>(jsonText);

            if (parsedData != null)
            {
                playerData = parsedData;   
            }
            else
            {
                Debug.LogWarning("Not able to parse player stats data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 데이터 파싱 중 오류 발생: {e.Message}");
        }
    }
    
}
