using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(fileName = "MiniGameSkillCollection", menuName = "Game/Data/MiniGame Skill Collection")]
public class MiniGameSkillScriptableObject : ScriptableObject
{
    public SerializedDictionary<Define.MiniGameSkillType, SkillData> MiniGameSkillData;
}
