using UnityEngine;

[CreateAssetMenu(fileName = "DurationSkillData", menuName = "Game/SkillData/DurationSkillData")]
public class DurationSkillData : SkillData
{
    public float MaxDuration = 10f;

    public int[] SkillDurationByLevel;

    public void SetLevel(int level)
    {
        MaxDuration = SkillDurationByLevel[level];    
    }
}
