using UnityEngine;

public enum MiniGameUnloadCharacterState
{

    
    // 캐릭터 행동
    HoldUp,
}

[RequireComponent(typeof(Animator))]                                            // 애니메이터 컴포넌트 필요
public class MiniGameUnloadCharacterAnimator : CharacterAnimator
{
    //~ 애니메이터 파라미터 이름 상수화
    private static readonly string PARAM_IS_COOLINGSKILL = "IsCoolingSkill"; // 냉동 스킬 상태 트리거

    private static readonly string PARAM_IS_HOLD_UP = "IsHoldUp"; // HoldUp 트리거

    //~ 냉동 스킬 상태
    public void SetCoolingSkill(bool isCooling)
    {
        animator.SetBool(PARAM_IS_COOLINGSKILL, isCooling); // 냉동 스킬 상태 파라미터 설정
    }

    //~ HoldUp 상태
    public void SetHoldUp(bool isHoldUp)
    {
        animator.SetBool(PARAM_IS_HOLD_UP, isHoldUp); // HoldUp 상태 파라미터 설정
    }
}