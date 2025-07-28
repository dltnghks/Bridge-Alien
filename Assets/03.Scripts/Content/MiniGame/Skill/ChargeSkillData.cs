using UnityEngine;

[CreateAssetMenu(fileName = "ChargeSkillData", menuName = "Game/SkillData/ChargeSkillData")]
public class ChargeSkillData : SkillData
{
    public int MaxCharges = 3;

    public int[] SkillChargesByLevel;

    public void SetLevel(int level)
    {
        MaxCharges = SkillChargesByLevel[level];    
    }
}