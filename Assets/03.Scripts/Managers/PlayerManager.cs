using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager
{
    private static readonly int[] StatThresholds = { 24, 49, 74, 100 };
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
            value = -GetFatigueReductionRate(playerGravityAdaptation);
        }

        PlayerData.Stats[type] += value;
        PlayerData.Stats[type] = Mathf.Clamp(PlayerData.Stats[type], 0, 100);

        OnPlayerDataChanged?.Invoke();


        OnPlayerDataChanged?.Invoke();
    }

    public float GetExperienceStatsBonus()
    {
        // 작업 숙련으로 인한 획득 골드 증가
        int playerExperience = PlayerData.Stats[Define.PlayerStatsType.Experience];
         if (playerExperience <= StatThresholds[0]) return GoldGainRates[0];
        if (playerExperience <= StatThresholds[1]) return GoldGainRates[1];
        if (playerExperience <= StatThresholds[2]) return GoldGainRates[2];
        return GoldGainRates[3];
    }

    public float GetFatigueStatsPenalty()
    {
        int playerFatigue = PlayerData.Stats[Define.PlayerStatsType.Fatigue];
        if (playerFatigue <= 0) return 0.9f;
        if (playerFatigue <= 30) return 0.5f;
        return 0;
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
    
    private int GetFatigueReductionRate(int gravityAdaptation)
    {
        if (gravityAdaptation <= StatThresholds[0]) return FatigueReductionRates[0];
        if (gravityAdaptation <= StatThresholds[1]) return FatigueReductionRates[1];
        if (gravityAdaptation <= StatThresholds[2]) return FatigueReductionRates[2];
        return FatigueReductionRates[3];
    }

    public int GetSkillLevel(Define.MiniGameSkillType skillType)
    {
        return PlayerData.MiniGameUnloadSkillLevel[skillType];
    }
    
    public bool UpgradeSkill(Define.MiniGameSkillType skillType, int gold)
    {
        if (PlayerData.PlayerGold < gold)
        {
            return false; // Not enough gold
        }

        PlayerData.PlayerGold -= gold;
        int maxLevel = Managers.Data.MiniGameSkillData.MiniGameSkillData[skillType].GetMaxLevel();
        if (PlayerData.MiniGameUnloadSkillLevel[skillType] < maxLevel)
        {
            PlayerData.MiniGameUnloadSkillLevel[skillType]++;
            OnPlayerDataChanged?.Invoke();
            return true;
        }
        return false;
    }
}
