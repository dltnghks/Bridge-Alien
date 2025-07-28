using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChargeSkill : SkillBase
{
    [SerializeField] protected ChargeSkillData skillData;
    [SerializeField] protected int remainingCharges;
    protected bool isActive;

    public ChargeSkillData SkillData => skillData;

    public Action<int> OnCountChanged;

    public override void Initialize(ISkillContext context)
    {
        int skillLevel = Managers.Player.PlayerData.MiniGameUnloadSkillLevel[skillData.Type];
        skillData.SetLevel(skillLevel);
        remainingCharges = skillData.MaxCharges;
        OnCountChanged?.Invoke(remainingCharges);
    }

    protected override void OnActivate()
    {
        isActive = true;
    }

    
    public override bool CanUseSkill() => remainingCharges > 0 && !isActive;
}