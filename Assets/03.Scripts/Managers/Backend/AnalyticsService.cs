using System;
using System.Collections.Generic;
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
        _lastFlushTime    = Time.realtimeSinceStartup;
    }

    // Managers.Update()에서 호출
    public void Update()
    {
        if (Time.realtimeSinceStartup - _lastFlushTime >= FlushIntervalSec)
            Flush();
    }

    // ── 이벤트 추적 ─────────────────────────────────────────────────────────

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
        Enqueue(Build("session_end", null, $"{{\"duration_sec\":{duration:F1}}}"));
        FlushSync(); // 앱 종료 시 코루틴이 실행되지 않으므로 동기 전송
    }

    public void TrackStageEnter(string stageId)
    {
        Enqueue(Build("stage_enter", stageId, null));
    }

    public void TrackStageClear(string stageId, int score, int starCount, float durationSec)
    {
        string payload = $"{{\"score\":{score},\"star_count\":{starCount},\"duration_sec\":{durationSec:F1}}}";
        Enqueue(Build("stage_clear", stageId, payload));
        Flush();
    }

    public void TrackStageFail(string stageId, int score, float durationSec)
    {
        string payload = $"{{\"score\":{score},\"duration_sec\":{durationSec:F1}}}";
        Enqueue(Build("stage_fail", stageId, payload));
        Flush();
    }

    public void TrackMinigameResult(string stageId, string minigameType, int score, float durationSec, bool success, int comboCount)
    {
        string successStr = success ? "true" : "false";
        string payload    = $"{{\"minigame_type\":\"{minigameType}\",\"score\":{score},\"duration_sec\":{durationSec:F1},\"success\":{successStr},\"combo_count\":{comboCount}}}";
        Enqueue(Build("minigame_result", stageId, payload));
        // Flush는 뒤이어 호출되는 TrackStageClear/TrackStageFail에서 담당
    }

    // ── 내부 ────────────────────────────────────────────────────────────────

    private AnalyticsEvent Build(string eventName, string stageId, string payloadJson) =>
        new AnalyticsEvent
        {
            PlayerId    = PlayerId,
            SessionId   = SessionId,
            EventName   = eventName,
            StageId     = stageId,
            PayloadJson = payloadJson,
            CreatedAt   = DateTime.UtcNow,
        };

    private void Enqueue(AnalyticsEvent e)
    {
        _queue.Add(e);
        if (_queue.Count >= MaxQueueSize)
            Flush();
    }

    public void Flush()
    {
#if !UNITY_EDITOR
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
#endif
    }

    private void FlushSync()
    {
#if !UNITY_EDITOR
        if (_queue.Count == 0) return;
        var batch = new List<AnalyticsEvent>(_queue);
        _queue.Clear();
        AnalyticsHttpClient.SendBatchSync(batch);
#endif
    }
}
