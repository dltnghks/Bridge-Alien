using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager
{
    public readonly float[] GoldGainRates = { 1f, 1.1f, 1.3f, 1.5f };
    public readonly float[] MoveSpeedBoostRates = { 1f, 1.05f, 1.05f, 1.1f };
    public readonly int[] FatigueReductionRates = { 30, 25, 20, 20 };
    
    public PlayerData PlayerData { get; private set; }

    public void Init(PlayerData playerData = null)
    {
        // 세이브 데이터 없으면 초기화
        if (playerData == null)
        {
            playerData = new PlayerData();
        }
        
        PlayerData = playerData;
    }

    public int GetStat(Define.PlayerStatType type)
    {
        return PlayerData.Stats[type];
    }
    
    public void AddStat(Define.PlayerStatType type, int value)
    {
        // 피로도 감소의 경우
        if (type == Define.PlayerStatType.Fatigue && value < 0)
        {
            int playerGravityAdaptation = PlayerData.Stats[Define.PlayerStatType.GravityAdaptation];
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
        
        
        PlayerData.Stats[type] += value;
        if (PlayerData.Stats[type] > 100)
        {
            PlayerData.Stats[type] = 100;
        }
        else if (PlayerData.Stats[type] < 0)
        {
            PlayerData.Stats[type] = 0;
        }
    }
    

    public float AddGold(float gold)
    {
        float totalGold = gold;

        
        // 피로도로 인한 획득 골드 감소
        int playerFatigue = PlayerData.Stats[Define.PlayerStatType.Fatigue];
        if (playerFatigue <= 0)
        {
            totalGold *= 0.9f;
        }else if (playerFatigue <= 30)
        {
            totalGold *= 0.5f;
        }
        
        // 작업 숙련으로 인한 획득 골드 증가
        int playerExperience = PlayerData.Stats[Define.PlayerStatType.Experience];
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
        int playerIntelligence = PlayerData.Stats[Define.PlayerStatType.Intelligence];
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
        
        PlayerData.PlayerGold += totalGold;
        
        return totalGold;
    }

    public void AddDate()
    {
        AddStat(Define.PlayerStatType.Fatigue, 50);
        AddStat(Define.PlayerStatType.Experience, 2);
        AddStat(Define.PlayerStatType.GravityAdaptation, 5);
    }

    public int GetGold()
    {
        throw new System.NotImplementedException();
    }
}
