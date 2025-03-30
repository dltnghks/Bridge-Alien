using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Data/PlayerStatData")]
public class PlayerDataScriptableObject : ScriptableObject
{
    public readonly float[] GoldGainRates = { 1f, 1.1f, 1.3f, 1.5f };
    public readonly float[] MoveSpeedBoostRates = { 1f, 1.05f, 1.05f, 1.1f };
    public readonly int[] FatigueReductionRates = { 30, 25, 20, 20 };
    
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
                Debug.LogWarning("Not able to parse player stat data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 데이터 파싱 중 오류 발생: {e.Message}");
        }
    }

    public int GetStat(Define.PlayerStatType type)
    {
        return playerData.Stats[type];
    }
    
    public void AddStat(Define.PlayerStatType type, int value)
    {
        // 피로도 감소의 경우
        if (type == Define.PlayerStatType.Fatigue && value < 0)
        {
            int playerGravityAdaptation = playerData.Stats[Define.PlayerStatType.GravityAdaptation];
            if (playerGravityAdaptation <= 24)
            {
                value = FatigueReductionRates[0];
            }
            else if(playerGravityAdaptation <= 49)
            {
                value = FatigueReductionRates[1];
            }
            else if (playerGravityAdaptation <= 74)
            {
                value = FatigueReductionRates[2];
            }
            else if (playerGravityAdaptation <= 100)
            {
                value = FatigueReductionRates[3];
            }
            value *= -1;
        }
        
        
        playerData.Stats[type] += value;
        if (playerData.Stats[type] > 100)
        {
            playerData.Stats[type] = 100;
        }
        else if (playerData.Stats[type] < 0)
        {
            playerData.Stats[type] = 0;
        }
    }
    

    public float AddGold(float gold)
    {
        float totalGold = gold;

        
        // 피로도로 인한 획득 골드 감소
        int playerFatigue = playerData.Stats[Define.PlayerStatType.Fatigue];
        if (playerFatigue <= 0)
        {
            totalGold *= 0.9f;
        }else if (playerFatigue <= 30)
        {
            totalGold *= 0.5f;
        }
        
        // 작업 숙련으로 인한 획득 골드 증가
        int playerExperience = playerData.Stats[Define.PlayerStatType.Experience];
        if (playerExperience <= 24)
        {
            totalGold *= GoldGainRates[0];
        }
        else if (playerExperience <= 49)
        {
            totalGold *= GoldGainRates[1];
        }
        else if (playerExperience <= 74)
        {
            totalGold *= GoldGainRates[2];
        }
        else if (playerExperience <= 100)
        {
            totalGold *= GoldGainRates[3];
        }
        
        // 지능으로 인한 획득 골드 증가
        int playerIntelligence = playerData.Stats[Define.PlayerStatType.Intelligence];
        if (playerIntelligence <= 24)
        {
            totalGold *= GoldGainRates[0];
        }
        else if (playerIntelligence <= 49)
        {
            totalGold *= GoldGainRates[1];
        }
        else if (playerIntelligence <= 74)
        {
            totalGold *= GoldGainRates[2];
        }
        else if (playerIntelligence <= 100)
        {
            totalGold *= GoldGainRates[3];
        }
        
        playerData.PlayerGold += totalGold;
        
        return totalGold;
    }

    public void AddDate()
    {
        AddStat(Define.PlayerStatType.Fatigue, 50);
        AddStat(Define.PlayerStatType.Experience, 2);
        AddStat(Define.PlayerStatType.GravityAdaptation, 5);
    }
}
