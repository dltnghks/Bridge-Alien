using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpSkill : DurationSkill, IRegainable
{
    private Player _playerCharacter; // 플레이어 캐릭터 참조
    private float speedBoost;

    private Action<bool> _onActiveAction;

    public void SetActiveAction(Action<bool> action)
    {
        _onActiveAction = action;
    }

    public void SetPlayerCharacter(Player player)
    {
        _playerCharacter = player;
        speedBoost = _playerCharacter.MoveSpeed * 0.1f; // 속도 증가량 설정
    }

    public void RegainResource(float amount)
    {
        if (isReady) return; // 활성화 중에는 리게인 불가

        currentDuration += Mathf.Min(currentDuration + amount, skillData.maxDuration);
        OnCooldownChanged?.Invoke(currentDuration, skillData.maxDuration);
        
        if (currentDuration >= skillData.maxDuration)
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
        isActive = true;
        currentDuration = skillData.maxDuration;
        _playerCharacter.SpeedUp(speedBoost); // 플레이어 속도 증가
        _onActiveAction?.Invoke(true); // 스킬 활성화 액션 호출
    }

    protected override void EndSkill()
    {
        base.EndSkill();
        _playerCharacter.SpeedUp(-speedBoost); // 플레이어 속도 초기화
        _onActiveAction?.Invoke(false); // 스킬 비활성화 액션 호출
    }

}
