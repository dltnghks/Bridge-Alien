using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class AnalyticsHttpClient
{
    public static IEnumerator SendBatch(List<AnalyticsEvent> events, Action onSuccess, Action onFailure)
    {
        if (events == null || events.Count == 0)
            yield break;

        var sb = new StringBuilder("[");
        for (int i = 0; i < events.Count; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append(events[i].ToJson());
        }
        sb.Append("]");

        byte[] body = Encoding.UTF8.GetBytes(sb.ToString());

        using var request = new UnityWebRequest(BackendApiConfig.AnalyticsEventsUrl, "POST");
        request.uploadHandler   = new UploadHandlerRaw(body);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader(BackendApiConfig.ApiKeyHeaderName, BackendApiConfig.TemporaryApiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Logger.Log($"Analytics: {events.Count}개 이벤트 전송 성공");
            onSuccess?.Invoke();
        }
        else
        {
            string responseBody = request.downloadHandler?.text ?? "";
            Logger.LogWarning($"Analytics: 전송 실패 (HTTP {(long)request.responseCode}, {request.error})");
            if (!string.IsNullOrEmpty(responseBody))
                Logger.LogWarning($"Analytics: 응답 본문 {responseBody}");
            Logger.LogWarning($"Analytics: 요청 본문 {sb}");
            onFailure?.Invoke();
        }
    }

    // OnApplicationQuit/OnApplicationPause용 동기 전송
    // 코루틴은 앱 종료 시 실행되지 않으므로 HttpClient로 블로킹 전송
    public static void SendBatchSync(List<AnalyticsEvent> events)
    {
        if (events == null || events.Count == 0) return;

        var sb = new StringBuilder("[");
        for (int i = 0; i < events.Count; i++)
        {
            if (i > 0) sb.Append(",");
            sb.Append(events[i].ToJson());
        }
        sb.Append("]");

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            using var request = new HttpRequestMessage(HttpMethod.Post, BackendApiConfig.AnalyticsEventsUrl)
            {
                Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Add(BackendApiConfig.ApiKeyHeaderName, BackendApiConfig.TemporaryApiKey);
            using HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                Logger.Log($"Analytics: session_end 동기 전송 완료");
                return;
            }

            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Logger.LogWarning($"Analytics: session_end 전송 실패 (HTTP {(int)response.StatusCode} {response.ReasonPhrase})");
            if (!string.IsNullOrEmpty(responseBody))
                Logger.LogWarning($"Analytics: session_end 응답 본문 {responseBody}");
            Logger.LogWarning($"Analytics: session_end 요청 본문 {sb}");
        }
        catch (Exception e)
        {
            Logger.LogWarning($"Analytics: session_end 전송 실패 ({e.Message})");
        }
    }
}
