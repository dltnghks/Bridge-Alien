using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AnalyticsService
{
    private const string PlayerIdKey = "analytics_player_id";
    private const float FlushIntervalSec = 30f;
    private const int MaxQueueSize = 50;

    public string PlayerId  { get; private set; }
    public string SessionId { get; private set; }

    private readonly List<AnalyticsEvent> _queue = new();
    private float _sessionStartTime;
    private float _lastFlushTime;
    private bool _sessionEnded;
    private string _currentStageId;
    private float _currentStageEnterTime;

    public void Init()
    {
        PlayerId = PlayerPrefs.GetString(PlayerIdKey, "");
        if (string.IsNullOrEmpty(PlayerId))
        {
            PlayerId = Guid.NewGuid().ToString();
            PlayerPrefs.SetString(PlayerIdKey, PlayerId);
            PlayerPrefs.Save();
        }

        SessionId = Guid.NewGuid().ToString();
        _sessionStartTime = Time.realtimeSinceStartup;
        _lastFlushTime = Time.realtimeSinceStartup;
        _currentStageId = null;
        _currentStageEnterTime = 0f;
    }

    public void Update()
    {
        if (Time.realtimeSinceStartup - _lastFlushTime >= FlushIntervalSec)
            Flush();
    }

    public void TrackSessionStart()
    {
        Enqueue(Build("session_start", null, null));
        Flush();
    }

    public void TrackSessionEnd()
    {
        if (_sessionEnded) return;
        _sessionEnded = true;

        float duration = Time.realtimeSinceStartup - _sessionStartTime;
        Enqueue(Build("session_end", null, $"{{\"duration_sec\":{FormatFloat(duration)}}}"));
        FlushSync();
    }

    public void TrackStageEnter(string stageId)
    {
        _currentStageId = stageId;
        _currentStageEnterTime = Time.realtimeSinceStartup;
        Enqueue(Build("stage_enter", stageId, null));
    }

    public void TrackStageClear(string stageId, int score, int starCount, float durationSec)
    {
        string payload = $"{{\"score\":{score},\"star_count\":{starCount},\"duration_sec\":{FormatFloat(durationSec)}}}";
        Enqueue(Build("stage_clear", stageId, payload));
        Flush();
    }

    public void TrackStageFail(string stageId, int score, float durationSec)
    {
        string payload = $"{{\"score\":{score},\"duration_sec\":{FormatFloat(durationSec)}}}";
        Enqueue(Build("stage_fail", stageId, payload));
        Flush();
    }

    public void TrackMinigameResult(string stageId, string minigameType, int score, float durationSec, bool success, int comboCount)
    {
        string successStr = success ? "true" : "false";
        string payload = $"{{\"minigame_type\":\"{EscapeJson(minigameType)}\",\"score\":{score},\"duration_sec\":{FormatFloat(durationSec)},\"success\":{successStr},\"combo_count\":{comboCount}}}";
        Enqueue(Build("minigame_result", stageId, payload));
    }

    public void TrackTaskExecute(
        string taskId,
        string taskName,
        string taskType,
        int requiredGold,
        int fatigueDelta,
        int experienceDelta,
        int strengthDelta,
        int gravityAdaptationDelta,
        int luckMin,
        int luckMax,
        int actualLuckDelta,
        int balanceAfter)
    {
        string payload =
            $"{{\"task_id\":\"{EscapeJson(taskId)}\",\"task_name\":\"{EscapeJson(taskName)}\",\"task_type\":\"{EscapeJson(taskType)}\",\"required_gold\":{requiredGold},\"fatigue_delta\":{fatigueDelta},\"experience_delta\":{experienceDelta},\"strength_delta\":{strengthDelta},\"gravity_adaptation_delta\":{gravityAdaptationDelta},\"luck_min\":{luckMin},\"luck_max\":{luckMax},\"actual_luck_delta\":{actualLuckDelta},\"balance_after\":{balanceAfter}}}";
        Enqueue(Build("task_execute", null, payload));
    }

    public void TrackSkillUse(string stageId, string minigameType, string skillType, int skillLevel, bool success, string failureReason)
    {
        float usedAtStageSec = 0f;
        if (!string.IsNullOrEmpty(stageId) && string.Equals(stageId, _currentStageId, StringComparison.Ordinal))
        {
            usedAtStageSec = Time.realtimeSinceStartup - _currentStageEnterTime;
        }

        string successStr = success ? "true" : "false";
        string payload =
            $"{{\"minigame_type\":\"{EscapeJson(minigameType)}\",\"skill_type\":\"{EscapeJson(skillType)}\",\"skill_level\":{skillLevel},\"success\":{successStr},\"failure_reason\":{ToJsonStringOrNull(failureReason)},\"used_at_stage_sec\":{FormatFloat(usedAtStageSec)}}}";
        Enqueue(Build("skill_use", stageId, payload));
    }

    public void TrackSkillUpgrade(string skillType, int prevLevel, int newLevel, int upgradeCost, int balanceAfter, bool success, string failureReason)
    {
        string successStr = success ? "true" : "false";
        string payload =
            $"{{\"skill_type\":\"{EscapeJson(skillType)}\",\"prev_level\":{prevLevel},\"new_level\":{newLevel},\"upgrade_cost\":{upgradeCost},\"balance_after\":{balanceAfter},\"success\":{successStr},\"failure_reason\":{ToJsonStringOrNull(failureReason)}}}";
        Enqueue(Build("skill_upgrade", null, payload));
    }

    public void TrackGoldChange(string stageId, int delta, int balanceAfter, string reason, string sourceId)
    {
        string payload =
            $"{{\"delta\":{delta},\"balance_after\":{balanceAfter},\"reason\":\"{EscapeJson(reason)}\",\"source_id\":{ToJsonStringOrNull(sourceId)}}}";
        Enqueue(Build("gold_change", stageId, payload));
    }

    private AnalyticsEvent Build(string eventName, string stageId, string payloadJson) =>
        new AnalyticsEvent
        {
            PlayerId = PlayerId,
            SessionId = SessionId,
            EventName = eventName,
            StageId = stageId,
            PayloadJson = payloadJson,
            CreatedAt = DateTime.UtcNow,
        };

    private void Enqueue(AnalyticsEvent e)
    {
        _queue.Add(e);
        if (_queue.Count >= MaxQueueSize)
            Flush();
    }

    private static string FormatFloat(float value)
    {
        return value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    private static string EscapeJson(string value)
    {
        return value?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";
    }

    private static string ToJsonStringOrNull(string value)
    {
        return string.IsNullOrEmpty(value) ? "null" : $"\"{EscapeJson(value)}\"";
    }

    public void Flush()
    {
        if (!GameConfigProvider.Config.EnableAnalytics)
        {
            _queue.Clear();
            return;
        }

        if (_queue.Count == 0) return;

        var batch = new List<AnalyticsEvent>(_queue);
        _queue.Clear();
        _lastFlushTime = Time.realtimeSinceStartup;

        Managers.Instance.StartCoroutine(
            AnalyticsHttpClient.SendBatch(
                batch,
                onSuccess: () => { },
                onFailure: () => _queue.InsertRange(0, batch)
            )
        );
    }

    private void FlushSync()
    {
        if (!GameConfigProvider.Config.EnableAnalytics)
        {
            _queue.Clear();
            return;
        }

        if (_queue.Count == 0) return;
        var batch = new List<AnalyticsEvent>(_queue);
        _queue.Clear();
        AnalyticsHttpClient.SendBatchSync(batch);
    }
}

