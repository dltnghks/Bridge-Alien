using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : ISaveable
{
    private const int FatigueMaxValue = 3;
    private static readonly int[] StatThresholds = { 24, 49, 74, 100 };
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
        int maxValue = 100;
        // 피로도 감소의 경우
        if (type == Define.PlayerStatsType.Fatigue)
        {
            maxValue = FatigueMaxValue;
        }

        PlayerData.Stats[type] += value;
        PlayerData.Stats[type] = Mathf.Clamp(PlayerData.Stats[type], 0, maxValue);

        OnPlayerDataChanged?.Invoke();
    }

    public void FillFatigue()
    {
        AddStats(Define.PlayerStatsType.Fatigue, FatigueMaxValue);
    }

    public float GetExperienceStatsBonus()
    {
        // 작업 숙련으로 인한 획득 점수 증가
        float playerExperience = PlayerData.Stats[Define.PlayerStatsType.Experience];
        playerExperience = (playerExperience / 10f) / 100f;
        return playerExperience;
    }

    public float GetFatigueStatsPenalty()
    {
        int playerFatigue = PlayerData.Stats[Define.PlayerStatsType.Fatigue];
        if (playerFatigue <= 0) return 0.9f;
        if (playerFatigue <= 30) return 0.5f;
        return 0;
    }
    public float GetGravityAdaptationBounus()
    {
        float gravityAdaptation = PlayerData.Stats[Define.PlayerStatsType.GravityAdaptation];
        // 50 => 1.25, 100 => 1.25 ...
        gravityAdaptation = (gravityAdaptation * 0.25f / 100f) + 1f;
        return gravityAdaptation;
    }

    public float AddGold(int gold)
    {
        PlayerData.PlayerGold += gold;

        PlayerData.PlayerGold = Math.Clamp(PlayerData.PlayerGold, 0, Int32.MaxValue);

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

        int maxLevel = Managers.Data.MiniGameSkillData.MiniGameSkillData[skillType].GetMaxLevel();
        if (PlayerData.MiniGameUnloadSkillLevel[skillType] < maxLevel)
        {
            PlayerData.MiniGameUnloadSkillLevel[skillType]++;
            OnPlayerDataChanged?.Invoke();
            AddGold(-gold);
            return true;
        }
        return false;
    }

    public void Add(ISaveable saveable)
    {
        throw new NotImplementedException();
    }

    public void SaveStageProgress(Define.ChapterType stageType, int star)
    {
        // 최고기록일 때만 갱신
        if (GetStageClearInfo(stageType) < star)
        {
            PlayerData.ClearedStages[stageType] = star;
            PlayerData.TotalStars += star;
        }
    }

    public int GetTotalStars()
    {
        return PlayerData.TotalStars;
    }

    public int GetCleardStageNum()
    {
        return PlayerData.ClearedStages.Count;
    }

    // 스테이지 클리어 정보 가져오기 - 별 개수
    public int GetStageClearInfo(Define.ChapterType stageType)
    {
        if (PlayerData.ClearedStages.ContainsKey(stageType))
        {
            return PlayerData.ClearedStages[stageType];
        }

        return 0;
    }

    // 해당 스테이지를 진행한 적이 있는가
    public bool GetStageProgressedStatus(Define.ChapterType stageType)
    {
        if (PlayerData.ClearedStages.ContainsKey(stageType))
        {
            return true;
        }
        return false;
    }

    public object CaptureState()
    {
        var data = new PlayerSaveData();
        data.PlayerData = PlayerData;
        return data;
    }

    public void RestoreState(object state)
    {
        var data = state as PlayerSaveData;
        if (data == null)
        {
            data = new PlayerSaveData();
            data.PlayerData = null;
            return;
        }
        Init(data.PlayerData);
    }
}
