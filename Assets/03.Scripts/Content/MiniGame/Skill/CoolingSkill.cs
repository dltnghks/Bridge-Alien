using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolingSkill : DurationSkill, IRegainable
{
    protected override void OnActivate()
    {
        isActive = true;
        currentDuration = skillData.maxDuration;
        // 캐릭터 외형 변경, 이펙트 활성화 로직
    }

    // 냉각 완료 상자를 배달했을 때 호출될 메서드
    public void RegainResource(float amount)
    {
        if (isReady) return; // 활성화 중에는 리게인 불가

        currentDuration = Mathf.Min(currentDuration + amount, skillData.maxDuration);
        OnCooldownChanged?.Invoke(currentDuration, skillData.maxDuration);

        if (currentDuration >= skillData.maxDuration)
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
        
        // 이펙트 추가
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