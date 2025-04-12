using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerTaskData
{
    public Define.TaskType TaskType;
    public string TaskName;
    public int RequirementGold;
    public int FatigueValue;
    public int ExperienceValue;
    public int GravityAdaptationValue;
    public int IntelligenceValue;
    public int LuckMaxValue;
    public int LuckMinValue;
}

