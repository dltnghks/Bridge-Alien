using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : ISaveable
{
    public const int FatigueMaxValue = 3;
    private static readonly int[] StatThresholds = { 24, 49, 74, 100 };
    public readonly int[] FatigueReductionRates = { 30, 25, 20, 20 };

    public PlayerData PlayerData { get; private set; }

    public UnityAction OnPlayerDataChanged { get; set; }
    private int _pendingFatigueRecoverEffectCount;

    public void Init(PlayerData playerData = null)
    {
        // 세이브 데이터 없으면 초기화
        if (playerData == null)
        {
            playerData = new PlayerData();
        }

        PlayerData = playerData;

        // DataManager가 준비된 경우 즉시 처리 (기존 유저 세이브 로드 시)
        if (Managers.Data != null && Managers.Data.MiniGameSkillData != null)
        {
            EnsureDefaultSkillLevels();
        }
    }

    // 해금 조건 없는 스킬은 기본 레벨 1 보장 (DataManager 준비 후 호출)
    public void EnsureDefaultSkillLevels()
    {
        foreach (var pair in Managers.Data.MiniGameSkillData.MiniGameSkillData)
        {
            if (!pair.Value.HasUnlockCondition && PlayerData.MiniGameUnloadSkillLevel[pair.Key] == 0)
            {
                UnlockSkill(pair.Key);
            }
        }
    }

    private void UnlockSkill(Define.MiniGameSkillType skillType)
    {
        PlayerData.MiniGameUnloadSkillLevel[skillType] = 1;
        OnPlayerDataChanged?.Invoke();
    }

    public int GetStats(Define.PlayerStatsType type)
    {
        return PlayerData.Stats[type];
    }

    public void AddStats(Define.PlayerStatsType type, int value)
    {
        int maxValue = 100;
        int previousValue = PlayerData.Stats[type];

        // 피로도 감소의 경우
        if (type == Define.PlayerStatsType.Fatigue)
        {
            maxValue = FatigueMaxValue;
        }

        PlayerData.Stats[type] += value;
        PlayerData.Stats[type] = Mathf.Clamp(PlayerData.Stats[type], 0, maxValue);

        if (type == Define.PlayerStatsType.Fatigue && PlayerData.Stats[type] > previousValue)
        {
            _pendingFatigueRecoverEffectCount += PlayerData.Stats[type] - previousValue;
        }

        OnPlayerDataChanged?.Invoke();
    }

    public int ConsumePendingFatigueRecoverEffectCount(int maxCount = int.MaxValue)
    {
        int count = Math.Min(_pendingFatigueRecoverEffectCount, maxCount);
        _pendingFatigueRecoverEffectCount -= count;
        return count;
    }

    public void FillFatigue()
    {
        AddStats(Define.PlayerStatsType.Fatigue, FatigueMaxValue);
    }

    public float GetExperienceStatsBonusRate()
    {
        // 작업 숙련 10당 미니게임 종료 보상 1% 증가
        return PlayerData.Stats[Define.PlayerStatsType.Experience] / 1000f;
    }

    public int GetExperienceStatsBonusPercent()
    {
        return Mathf.FloorToInt(GetExperienceStatsBonusRate() * 100f);
    }

    public float GetFatigueStatsPenalty()
    {
        int playerFatigue = PlayerData.Stats[Define.PlayerStatsType.Fatigue];
        if (playerFatigue <= 0) return 0.9f;
        if (playerFatigue <= 30) return 0.5f;
        return 0;
    }
    public float GetGravityAdaptationBonusMultiplier()
    {
        return (PlayerData.Stats[Define.PlayerStatsType.GravityAdaptation] * 0.25f / 100f) + 1f;
    }

    public int GetGravityAdaptationBonusPercent()
    {
        return Mathf.FloorToInt((GetGravityAdaptationBonusMultiplier() - 1f) * 100f);
    }

    public int GetStrengthBonusPercent()
    {
        return PlayerData.Stats[Define.PlayerStatsType.Strength];
    }

    public int GetStrengthMoveSpeedReductionDecreasePercent()
    {
        return Mathf.FloorToInt(PlayerData.Stats[Define.PlayerStatsType.Strength] / 20f);
    }

    public float GetStrengthAdjustedMoveSpeedReductionRatio(float baseReductionRatioPerBox)
    {
        return Mathf.Max(0f, baseReductionRatioPerBox - GetStrengthMoveSpeedReductionDecreasePercent());
    }

    public int GetLuckBonusPercent()
    {
        return Mathf.FloorToInt(PlayerData.Stats[Define.PlayerStatsType.Luck] / 10f);
    }

    public int GetLuckyBonusTotalChancePercent()
    {
        return 5 + GetLuckBonusPercent();
    }

    public float AddGold(int gold, string reason = "unknown", string sourceId = null, string stageId = null)
    {
        PlayerData.PlayerGold += gold;

        PlayerData.PlayerGold = Math.Clamp(PlayerData.PlayerGold, 0, Int32.MaxValue);
        if (gold > 0)
        {
            PlayerData.TotalEarnedGold = Math.Clamp(PlayerData.TotalEarnedGold + gold, 0, Int32.MaxValue);
        }
        else if (gold < 0)
        {
            PlayerData.TotalSpentGold = Math.Clamp(PlayerData.TotalSpentGold + Math.Abs(gold), 0, Int32.MaxValue);
        }

        OnPlayerDataChanged?.Invoke();

        if (gold != 0)
        {
            Managers.Analytics.TrackGoldChange(stageId, gold, PlayerData.PlayerGold, reason, sourceId);
        }

        return gold;
    }

    public void AddDeliveredBoxCount(int count = 1)
    {
        AddEndingStat(ref PlayerData.DeliveredBoxCount, count);
    }

    public void AddDisposedBoxCount(int count = 1)
    {
        AddEndingStat(ref PlayerData.DisposedBoxCount, count);
    }

    public void AddFailedDeliveryBoxCount(int count = 1)
    {
        AddEndingStat(ref PlayerData.FailedDeliveryBoxCount, count);
    }

    public void AddStageAttemptCount(int count = 1)
    {
        AddEndingStat(ref PlayerData.StageAttemptCount, count);
    }

    public void AddStageFailCount(int count = 1)
    {
        AddEndingStat(ref PlayerData.StageFailCount, count);
    }

    private void AddEndingStat(ref int stat, int count)
    {
        if (count <= 0)
        {
            return;
        }

        stat = Math.Clamp(stat + count, 0, Int32.MaxValue);
        OnPlayerDataChanged?.Invoke();
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
        int prevLevel = PlayerData.MiniGameUnloadSkillLevel[skillType];

        if (PlayerData.PlayerGold < gold)
        {
            Managers.Analytics.TrackSkillUpgrade(skillType.ToString(), prevLevel, prevLevel, gold, PlayerData.PlayerGold, false, "insufficient_gold");
            return false;
        }

        int maxLevel = Managers.Data.MiniGameSkillData.MiniGameSkillData[skillType].GetMaxLevel();
        if (prevLevel < maxLevel)
        {
            int newLevel = prevLevel + 1;
            PlayerData.MiniGameUnloadSkillLevel[skillType] = newLevel;
            OnPlayerDataChanged?.Invoke();
            AddGold(-gold, "skill_upgrade", skillType.ToString());
            Managers.Analytics.TrackSkillUpgrade(skillType.ToString(), prevLevel, newLevel, gold, PlayerData.PlayerGold, true, null);
            Managers.Analytics.Flush();
            return true;
        }

        Managers.Analytics.TrackSkillUpgrade(skillType.ToString(), prevLevel, prevLevel, gold, PlayerData.PlayerGold, false, "max_level");
        return false;
    }

    public void Add(ISaveable saveable)
    {
        throw new NotImplementedException();
    }

    public void SaveStageProgress(Define.ChapterType stageType, int star)
    {
        int previousStar = GetStageClearInfo(stageType);

        // 최고기록일 때만 갱신
        if (previousStar < star)
        {
            PlayerData.ClearedStages[stageType] = star;
            PlayerData.TotalStars += (star - previousStar);
        }

        // 최초 클리어 시 해당 스테이지 해금 조건 스킬에 레벨 1 부여
        if (previousStar == 0 && star > 0)
        {
            UnlockSkillsByStage(stageType);
        }
    }

    private void UnlockSkillsByStage(Define.ChapterType stageType)
    {
        foreach (var pair in Managers.Data.MiniGameSkillData.MiniGameSkillData)
        {
            var skillData = pair.Value;
            if (skillData.HasUnlockCondition && skillData.UnlockStage == stageType
                && PlayerData.MiniGameUnloadSkillLevel[pair.Key] == 0)
            {
                UnlockSkill(pair.Key);
            }
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

    public void SaveStageProgressed(Define.ChapterType stageType)
    {
        if (PlayerData.ProgressedStages == null)
        {
            PlayerData.ProgressedStages = new List<Define.ChapterType>();
        }

        if (PlayerData.ProgressedStages.Contains(stageType))
        {
            return;
        }

        PlayerData.ProgressedStages.Add(stageType);
        OnPlayerDataChanged?.Invoke();
    }

    // 해당 스테이지를 진행한 적이 있는가
    public bool GetStageProgressedStatus(Define.ChapterType stageType)
    {
        if (PlayerData.ProgressedStages == null)
        {
            PlayerData.ProgressedStages = new List<Define.ChapterType>();
        }

        if (PlayerData.ProgressedStages.Contains(stageType))
        {
            return true;
        }

        // 구세이브 호환: 이전 버전에서는 클리어 정보만 진행 판정에 사용
        if (PlayerData.ClearedStages.ContainsKey(stageType))
        {
            return true;
        }

        return false;
    }

    public void SaveEndingThumbnailProgress(Define.ChapterType stageType, int thumbnailIndex)
    {
        if (thumbnailIndex < 0)
        {
            return;
        }

        if (PlayerData.ClearedEndingThumbnailMask == null)
        {
            PlayerData.ClearedEndingThumbnailMask = new Dictionary<Define.ChapterType, int>();
        }

        int previousMask = 0;
        if (PlayerData.ClearedEndingThumbnailMask.TryGetValue(stageType, out int savedMask))
        {
            previousMask = savedMask;
        }
        else if (PlayerData.ClearedEndingThumbnailIndex != null && PlayerData.ClearedEndingThumbnailIndex.TryGetValue(stageType, out int legacyIndex))
        {
            previousMask = 1 << Mathf.Clamp(legacyIndex, 0, 30);
        }

        int currentMask = previousMask | (1 << Mathf.Clamp(thumbnailIndex, 0, 30));
        if (currentMask == previousMask)
        {
            return;
        }

        PlayerData.ClearedEndingThumbnailMask[stageType] = currentMask;
        if (PlayerData.ClearedEndingThumbnailIndex == null)
        {
            PlayerData.ClearedEndingThumbnailIndex = new Dictionary<Define.ChapterType, int>();
        }
        PlayerData.ClearedEndingThumbnailIndex[stageType] = thumbnailIndex;
        OnPlayerDataChanged?.Invoke();
    }

    public int GetEndingThumbnailProgress(Define.ChapterType stageType)
    {
        if (IsEndingThumbnailUnlocked(stageType, 1))
        {
            return 1;
        }

        if (IsEndingThumbnailUnlocked(stageType, 0))
        {
            return 0;
        }

        return -1;
    }

    public bool IsEndingThumbnailUnlocked(Define.ChapterType stageType, int thumbnailIndex)
    {
        if (thumbnailIndex < 0)
        {
            return false;
        }

        int mask = GetEndingThumbnailMask(stageType);
        int bit = 1 << Mathf.Clamp(thumbnailIndex, 0, 30);
        return (mask & bit) != 0;
    }

    public int GetEndingUnlockedCount(Define.ChapterType stageType, int endingTypeCount)
    {
        int count = 0;
        for (int i = 0; i < endingTypeCount; i++)
        {
            if (IsEndingThumbnailUnlocked(stageType, i))
            {
                count++;
            }
        }

        return count;
    }

    private int GetEndingThumbnailMask(Define.ChapterType stageType)
    {
        if (PlayerData.ClearedEndingThumbnailMask == null)
        {
            PlayerData.ClearedEndingThumbnailMask = new Dictionary<Define.ChapterType, int>();
        }

        if (PlayerData.ClearedEndingThumbnailMask.TryGetValue(stageType, out int mask))
        {
            return mask;
        }

        if (PlayerData.ClearedEndingThumbnailIndex == null)
        {
            PlayerData.ClearedEndingThumbnailIndex = new Dictionary<Define.ChapterType, int>();
        }

        if (PlayerData.ClearedEndingThumbnailIndex.TryGetValue(stageType, out int legacyIndex))
        {
            return 1 << Mathf.Clamp(legacyIndex, 0, 30);
        }

        return 0;
    }

    public bool HasSeenEvent(Define.EventDataID eventId)
    {
        if (eventId == Define.EventDataID.Unknown)
        {
            return true;
        }

        if (PlayerData.SeenEvents == null)
        {
            PlayerData.SeenEvents = new List<Define.EventDataID>();
        }

        return PlayerData.SeenEvents.Contains(eventId);
    }

    public void MarkEventSeen(Define.EventDataID eventId)
    {
        if (eventId == Define.EventDataID.Unknown)
        {
            return;
        }

        if (PlayerData.SeenEvents == null)
        {
            PlayerData.SeenEvents = new List<Define.EventDataID>();
        }

        if (!PlayerData.SeenEvents.Contains(eventId))
        {
            PlayerData.SeenEvents.Add(eventId);
            OnPlayerDataChanged?.Invoke();
        }
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
