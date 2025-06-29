using UnityEngine;

// 시간 기반 스킬 데이터
[CreateAssetMenu(fileName = "DurationSkillData", menuName = "Skills/Duration Skill")]
public class DurationSkillData : SkillData
{
    public float maxDuration = 10f;
}
