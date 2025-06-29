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

    protected virtual void Awake()
    {
        remainingCharges = skillData.maxCharges;
    }

    public void SetCountChangedAction(Action<int> action)
    {
        OnCountChanged += action;
    }
    
    public override bool CanUseSkill() => remainingCharges > 0 && !isActive;
}