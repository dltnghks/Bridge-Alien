using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour
{
    protected bool isReady = true;

    public abstract void TryActivate();     // 외부 호출 메서드
    protected abstract void OnActivate();   // 각 스킬 구현 필수
    public abstract bool CanUseSkill();     // 사용 가능 조건
}