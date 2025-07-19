using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolingSkill : DurationSkill, IRegainable
{
    private Action<bool> _onSkillAction;

    public override void Initialize(MGUSkillContext context)
    {
        _onSkillAction = context.SetCoolingSkillAction;
        
        base.Initialize(context); // 부모 클래스의 초기화 호출
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        OnActiveStateChanged?.Invoke(true);
        currentDuration = skillData.MaxDuration;
        // 캐릭터 외형 변경, 이펙트 활성화 로직
        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.CoolingSkill.ToString(), gameObject);
    }

    protected override void EndSkill()
    {
        base.EndSkill();
    }

    // 냉각 완료 상자를 배달했을 때 호출될 메서드
    public void RegainResource(float amount)
    {
        if (isReady) return; // 활성화 중에는 리게인 불가

        currentDuration = Mathf.Min(currentDuration + amount, skillData.MaxDuration);
        OnCooldownChanged?.Invoke(currentDuration, skillData.MaxDuration);

        if (currentDuration >= skillData.MaxDuration)
        {
            isReady = true; // 스킬 재사용 가능
        }
    }

    // 상자를 집었을 때의 처리 로직 (이벤트 등으로 호출)
    public void OnPickUpBox(MiniGameUnloadBox box)
    {
        if (!isActive) return;

        if (box is ColdBox coldBox)
        {
            coldBox.DirectCooling();
        }


    }

    public override void TryActivate()
    {
        Logger.Log("BoxWarpSkill TryActivate");
        if (CanUseSkill())
        {
            OnActivate();
            isReady = false; // 스킬 사용 후 재사용 불가 상태로 변경
        }
        else
        {
            Logger.Log("CoolingSkill cannot be activated");
        }
    }

}