using System;
using System.Text;
using UnityEngine;

public class AnalyticsEvent
{
    public string PlayerId;
    public string SessionId;
    public string EventName;
    public string StageId;
    public string PayloadJson; // JSON object string, e.g. {"score":1200}
    public DateTime CreatedAt;

    public string ToJson()
    {
        var sb = new StringBuilder();
        sb.Append("{");
        sb.Append($"\"player_id\":\"{Escape(PlayerId)}\",");
        sb.Append($"\"session_id\":\"{Escape(SessionId)}\",");
        sb.Append($"\"event_name\":\"{Escape(EventName)}\",");

        if (StageId != null)
            sb.Append($"\"stage_id\":\"{Escape(StageId)}\",");
        else
            sb.Append("\"stage_id\":null,");

        sb.Append($"\"payload\":{(string.IsNullOrEmpty(PayloadJson) ? "null" : PayloadJson)},");
        sb.Append($"\"client_version\":\"{Escape(Application.version)}\",");
        sb.Append($"\"platform\":\"{Escape(Application.platform.ToString())}\",");
        sb.Append($"\"created_at\":\"{CreatedAt:yyyy-MM-ddTHH:mm:ssZ}\"");
        sb.Append("}");
        return sb.ToString();
    }

    private static string Escape(string s) =>
        s?.Replace("\\", "\\\\").Replace("\"", "\\\"") ?? "";
}
