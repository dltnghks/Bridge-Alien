using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpSkill : DurationSkill, IRegainable
{
    private Player _playerCharacter; // 플레이어 캐릭터 참조
    private float speedBoost;

    public void SetPlayerCharacter(Player player)
    {
        _playerCharacter = player;
        speedBoost = _playerCharacter.MoveSpeed * 0.1f; // 속도 증가량 설정
    }

    public void RegainResource(float amount)
    {
        if (isActive) return; // 활성화 중에는 리게인 불가

        currentDuration = Mathf.Min(currentDuration + amount, 2);

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
        // TODO. 10% 상승하는 걸로 바꿔야 됨.
        _playerCharacter.SpeedUp(speedBoost); // 플레이어 속도 증가
    }

    protected override void EndSkill()
    {
        base.EndSkill();
        _playerCharacter.SpeedUp(-speedBoost); // 플레이어 속도 초기화
    }

}
