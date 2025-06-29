using UnityEngine;

// 횟수 기반 스킬 데이터
[CreateAssetMenu(fileName = "ChargeSkillData", menuName = "Skills/Charge Skill")]
public class ChargeSkillData : SkillData
{
    public int maxCharges = 3;
}