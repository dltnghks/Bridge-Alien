using UnityEngine;

public enum MGDCharacterState
{
    Move,
    Crash,
    Hit,
}

[RequireComponent(typeof(Animator))]                                            // 애니메이터 컴포넌트 필요
public class MGDCharacterAnimator : CharacterAnimator
{
    private static readonly string PARAM_IS_CRASH = "CRASH";
    private static readonly string PARAM_IS_HIT = "HIT";

    public void SetCrash(bool isCrashed)
    {
        animator.SetBool(PARAM_IS_CRASH, isCrashed);
    }

    public void SetHit(bool isHited)
    {
        animator.SetBool(PARAM_IS_HIT, isHited);
    }
}
