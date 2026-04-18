using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Config/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Backend")]
    public bool EnableAnalytics = true;

    [Header("Test Build")]
    public bool UnlockAllStagesForTest = false;
}
