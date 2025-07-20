using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DurationSkill : SkillBase
{
    [SerializeField] protected DurationSkillData skillData;
    [SerializeField] protected float currentDuration;
    protected bool isActive = false;

    public DurationSkillData SkillData => skillData;

    // 쿨타임이 시작될 때와 진행 중일 때를 알리기 위한 이벤트
    public Action<float, float> OnCooldownChanged;
    public Action<bool> OnActiveStateChanged;

    public override void Initialize(ISkillContext context)
    {
        int skillLevel = Managers.Player.PlayerData.MiniGameUnloadSkillLevel[skillData.Type];
        skillData.SetLevel(skillLevel);
        currentDuration = skillData.MaxDuration;
        OnCooldownChanged?.Invoke(currentDuration, skillData.MaxDuration);
    }

    protected virtual void Update()
    {
        if (isActive && !Managers.MiniGame.CurrentGame.IsPause)
        {
            currentDuration -= Time.deltaTime;
            OnCooldownChanged?.Invoke(currentDuration, skillData.MaxDuration);

            if (currentDuration <= 0)
            {
                EndSkill();
            }
        }
    }

    protected override void OnActivate()
    {
        isActive = true;
        OnActiveStateChanged?.Invoke(true);
    }

    protected virtual void EndSkill()
    {
        isActive = false;
        isReady = false;
        currentDuration = 0;
        OnCooldownChanged?.Invoke(0, skillData.MaxDuration);
        OnActiveStateChanged?.Invoke(false);
        // 스킬 종료 후 필요한 로직
    }

    public override bool CanUseSkill() => isReady && !isActive;
}
