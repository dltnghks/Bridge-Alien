using UnityEngine;

[System.Serializable]
public abstract class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public Define.MiniGameSkillType Type;
    public string Name;
    public Sprite Icon;
    public string Description;
    public int[] UpgradeCostByLevel;
}
