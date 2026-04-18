using UnityEngine;

public static class GameConfigProvider
{
    private const string AppConfigResourcePath = "Config/AppConfig";
    private static GameConfig _config;

    public static GameConfig Config
    {
        get
        {
            if (_config != null)
            {
                return _config;
            }

            var appConfig = Resources.Load<AppConfig>(AppConfigResourcePath);
            if (appConfig != null && appConfig.ActiveGameConfig != null)
            {
                _config = appConfig.ActiveGameConfig;
                return _config;
            }

            _config = ScriptableObject.CreateInstance<GameConfig>();
            Logger.LogWarning($"AppConfig asset not found at Resources/{AppConfigResourcePath} or ActiveGameConfig is missing. Using runtime defaults.");
            return _config;
        }
    }

    public static void ResetCache()
    {
        _config = null;
    }
}
