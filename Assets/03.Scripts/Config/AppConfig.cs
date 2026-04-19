using UnityEngine;

[CreateAssetMenu(fileName = "AppConfig", menuName = "Game/Config/AppConfig")]
public class AppConfig : ScriptableObject
{
    [Header("Active Config")]
    public GameConfig ActiveGameConfig;
}
