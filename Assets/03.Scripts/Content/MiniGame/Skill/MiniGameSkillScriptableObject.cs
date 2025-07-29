using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "MiniGameSkillData", menuName = "Game/Data/MiniGame Skill Collection")]
public class MiniGameSkillScriptableObject : ScriptableObject
{
    public SerializedDictionary<Define.MiniGameSkillType, SkillData> MiniGameSkillData;

    public List<SkillData> GetMiniGameSkillList(Define.MiniGameType miniGameType)
    {
        Logger.Log("GetMiniGameSkillList");
        List<SkillData> skillList = new List<SkillData>();

        switch (miniGameType)
        {
            case Define.MiniGameType.Unload:
                skillList.Add(MiniGameSkillData[Define.MiniGameSkillType.CoolingSkill]);
                skillList.Add(MiniGameSkillData[Define.MiniGameSkillType.BoxWarpSkill]);
                skillList.Add(MiniGameSkillData[Define.MiniGameSkillType.SpeedUpSkill]);
                break;
            
            case Define.MiniGameType.Delivery:
                skillList.Add(MiniGameSkillData[Define.MiniGameSkillType.EmergencyRepairSkill]);
                skillList.Add(MiniGameSkillData[Define.MiniGameSkillType.EmergencyRocketSkill]);
                break;
        }

        return skillList;
    }

    public SkillData GetSkillData(Define.MiniGameSkillType skillType)
    {
        SkillData skillData = null;
        MiniGameSkillData.TryGetValue(skillType, out skillData);

        if (skillData == null)
        {
            Logger.LogError("잘못된 스킬 타입 접근");
        }

        return skillData;
    }
}
