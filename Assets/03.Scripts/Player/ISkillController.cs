using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkillController
{
    public SkillBase[] SkillList { get; set; }
    public void SetSkillList(SkillBase[] skillList);

    public void OnSkill(int skillIndex);
}
