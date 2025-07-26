using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager
{
    public readonly float[] GoldGainRates = { 1f, 1.1f, 1.3f, 1.5f };
    public readonly float[] MoveSpeedBoostRates = { 1f, 1.05f, 1.05f, 1.1f };
    public readonly int[] FatigueReductionRates = { 30, 25, 20, 20 };
    
    public PlayerData PlayerData { get; private set; }

    public UnityAction OnPlayerDataChanged { get; set; }
    
    public void Init(PlayerData playerData = null)
    {
        // 세이브 데이터 없으면 초기화
        if (playerData == null)
        {
            playerData = new PlayerData();
        }
        
        PlayerData = playerData;
    }
    
    public int GetStats(Define.PlayerStatsType type)
    {
        return PlayerData.Stats[type];
    }
    
    public void AddStats(Define.PlayerStatsType type, int value)
    {
        // 피로도 감소의 경우
        if (type == Define.PlayerStatsType.Fatigue && value < 0)
        {
            int playerGravityAdaptation = PlayerData.Stats[Define.PlayerStatsType.GravityAdaptation];
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
        
        OnPlayerDataChanged?.Invoke();
    }

    public float GetExperienceStatsBonus()
    {

        // 작업 숙련으로 인한 획득 골드 증가
        int playerExperience = PlayerData.Stats[Define.PlayerStatsType.Experience];
        if (playerExperience <= 24)
        {
            return GoldGainRates[0];
        }
        else if (playerExperience <= 49)
        {
            return GoldGainRates[1];
        }
        else if (playerExperience <= 74)
        {
            return GoldGainRates[2];
        }
        else if( playerExperience <= 100)
        {
            return GoldGainRates[3];
        }

        return 1f;
    }

    public float GetFatigueStatsPenalty()
    {
        int playerFatigue = PlayerData.Stats[Define.PlayerStatsType.Fatigue];
        if (playerFatigue <= 0)
        {
            return 0.9f;
        }
        else if (playerFatigue < 30)
        {
            return 0.5f;
        }

        return 0f;
    }

    public float AddGold(int gold)
    {
        PlayerData.PlayerGold += gold;

        OnPlayerDataChanged?.Invoke();

        return gold;
    }

    public void AddDate()
    {
        AddStats(Define.PlayerStatsType.Fatigue, 50);
        AddStats(Define.PlayerStatsType.Experience, 2);
        AddStats(Define.PlayerStatsType.GravityAdaptation, 5);
    }

    public int GetGold()
    {
        return PlayerData.PlayerGold;
    }
}
