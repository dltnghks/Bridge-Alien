using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyRepairSkill : DurationSkill, IRegainable
{
    public override void Initialize(ISkillContext context)
    {
        var skillContext = context as MGDSkillContext;
        OnActiveStateChanged += skillContext.OnRepairSkillAction;
        
        base.Initialize(context);
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.RepairSkill.ToString());

        currentDuration = skillData.MaxDuration;
        EndSkill();
    }

    protected override void EndSkill()
    {
        base.EndSkill();
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
