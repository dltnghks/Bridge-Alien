using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyRocketSkill : DurationSkill, IRegainable
{
    public override void Initialize(ISkillContext context)
    {
        var skillContext = context as MGDSkillContext;
        OnActiveStateChanged += skillContext.OnRocketSkillAction;
        
        base.Initialize(context);
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        
        OnActiveStateChanged?.Invoke(true);
        currentDuration = skillData.MaxDuration;
        
        // Skill SFX
    }

    protected override void EndSkill()
    {
        base.EndSkill();
        OnActiveStateChanged?.Invoke(false);
    }
    
    public override void TryActivate()
    {
        if (CanUseSkill())
        {
            OnActivate();
            isReady = false;
        }
    }
    public void RegainResource(float amount)
    {
        throw new System.NotImplementedException();
    }
}
