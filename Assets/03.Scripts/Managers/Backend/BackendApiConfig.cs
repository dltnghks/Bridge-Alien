public static class BackendApiConfig
{
    public const string AnalyticsEventsUrl = "https://bridge-alien-analytics-production.up.railway.app/analytics/events";

    // Temporary client key for internal testing only.
    // Do not treat this as a production security control.
    public const string TemporaryApiKey = "asdfzxcvqwer123";
    public const string ApiKeyHeaderName = "X-Api-Key";
}
