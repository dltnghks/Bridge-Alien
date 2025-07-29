using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyRepairSkill : DurationSkill, IRegainable
{
    public override void Initialize(ISkillContext context)
    {
        var skillContext = context as MGDSkillContext;
        
        base.Initialize(context);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        
        currentDuration = skillData.MaxDuration;
    }

    protected override void EndSkill()
    {
        base.EndSkill();
    }
    
    public override void TryActivate()
    {
        throw new System.NotImplementedException();
    }
    public void RegainResource(float amount)
    {
        throw new System.NotImplementedException();
    }
}
