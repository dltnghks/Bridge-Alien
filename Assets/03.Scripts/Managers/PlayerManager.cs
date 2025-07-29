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

    public int GetStat(Define.PlayerStatsType type)
    {
        return PlayerData.Stats[type];
    }

    public void AddStat(Define.PlayerStatsType type, int value)
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
    }

    public float AddGold(int gold)
    {
        float totalGold = gold;

        // 피로도로 인한 획득 골드 감소
        totalGold *= GetFatiguePenaltyRate(PlayerData.Stats[Define.PlayerStatsType.Fatigue]);

        // 작업 숙련으로 인한 획득 골드 증가
        totalGold *= GetGoldGainRateByStat(PlayerData.Stats[Define.PlayerStatsType.Experience]);

        // 지능으로 인한 획득 골드 증가
        totalGold *= GetGoldGainRateByStat(PlayerData.Stats[Define.PlayerStatsType.Intelligence]);

        PlayerData.PlayerGold += (int)totalGold;

        OnPlayerDataChanged?.Invoke();

        return totalGold;
    }

    private float GetGoldGainRateByStat(int statValue)
    {
        if (statValue <= StatThresholds[0]) return GoldGainRates[0];
        if (statValue <= StatThresholds[1]) return GoldGainRates[1];
        if (statValue <= StatThresholds[2]) return GoldGainRates[2];
        return GoldGainRates[3];
    }

    private float GetFatiguePenaltyRate(int fatigueValue)
    {
        if (fatigueValue <= 0) return 0.9f;
        if (fatigueValue <= 30) return 0.5f;
        return 1f;
    }
    
    private int GetFatigueReductionRate(int gravityAdaptation)
    {
        if (gravityAdaptation <= StatThresholds[0]) return FatigueReductionRates[0];
        if (gravityAdaptation <= StatThresholds[1]) return FatigueReductionRates[1];
        if (gravityAdaptation <= StatThresholds[2]) return FatigueReductionRates[2];
        return FatigueReductionRates[3];
    }

    public void AddDate()
    {
        AddStat(Define.PlayerStatsType.Fatigue, 50);
        AddStat(Define.PlayerStatsType.Experience, 2);
        AddStat(Define.PlayerStatsType.GravityAdaptation, 5);
    }

    public int GetGold()
    {
        return PlayerData.PlayerGold;
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
        int maxLevel = Managers.Data.MiniGameSkillData.MiniGameSkillData[skillType].UpgradeCostByLevel.Length - 1;
        if (PlayerData.MiniGameUnloadSkillLevel[skillType] < maxLevel)
        {
            PlayerData.MiniGameUnloadSkillLevel[skillType]++;
            OnPlayerDataChanged?.Invoke();
            return true;
        }
        return false;
    }
}
