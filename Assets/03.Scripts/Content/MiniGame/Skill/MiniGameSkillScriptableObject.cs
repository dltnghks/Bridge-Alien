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
                //skillList.Add(MiniGameSkillData[Define.MiniGameSkillType.TestSkill]);
                break;
        }

        return skillList;
    }
}
