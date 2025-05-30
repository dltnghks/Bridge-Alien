using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerTaskData
{
    // 일과 정보
    public string TaskID;
    public string TaskName;
    public string ThumbnailText;
    public string TaskProgressText;
    public string TaskCompletedText;
    
    // 일과에 요구되는 골드량
    public int RequirementGold;
    
    // 일과 수행 시 증가량
    public int FatigueValue;
    public int ExperienceValue;
    public int GravityAdaptationValue;
    public int IntelligenceValue;
    public int LuckMaxValue;
    public int LuckMinValue;
}

