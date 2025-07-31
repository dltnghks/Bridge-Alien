using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpSkill : DurationSkill, IRegainable
{
    private Player _playerCharacter; // 플레이어 캐릭터 참조
    private float speedBoost;

    public override void Initialize(ISkillContext context)
    {
        MGUSkillContext mguSkillContext = context as MGUSkillContext;

        _playerCharacter = mguSkillContext.Player;
        speedBoost = _playerCharacter.MoveSpeed * 0.1f; // 속도 증가량 설정
        OnActiveStateChanged += mguSkillContext.SetSpeedUpSkillAction;

        base.Initialize(context); // 부모 클래스의 초기화 호출
    }

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

    public override void TryActivate()
    {
        Logger.Log("SpeedUpSkill TryActivate");
        if (CanUseSkill())
        {
            OnActivate();
        }
        else
        {
            Logger.Log("SpeedUpSkill cannot be activated, conditions not met.");
        }
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        OnActiveStateChanged?.Invoke(true);
        currentDuration = skillData.MaxDuration;
        _playerCharacter.SpeedUp(speedBoost); // 플레이어 속도 증가

        Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.SpeedUpSkill.ToString(), gameObject);
    }

    protected override void EndSkill()
    {
        base.EndSkill();
        _playerCharacter.SpeedUp(-speedBoost); // 플레이어 속도 초기화
    }

}
