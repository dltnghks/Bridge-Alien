using UnityEngine;

/// <summary>
/// HousePlayer의 애니메이션을 제어합니다. (걷기/쉬기)
/// </summary>
[RequireComponent(typeof(Animator))]
public class HousePlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    protected static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    protected static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    protected static readonly string PARAM_IS_REST = "IsRest";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 이동 속도에 따라 애니메이션 파라미터를 업데이트합니다.
    /// </summary>
    /// <param name="speed">현재 이동 속도</param>
    public void UpdateSpeed(float speed)
    {
        if (!_animator) return;

        // 이동 여부와 속도를 애니메이터에 전달
        _animator.SetBool(IsMovingHash, speed > 0.001f);
        _animator.SetFloat(MoveSpeedHash, speed);
    }

    public void Rest(bool value)
    {
        if (!_animator) return;

        _animator.SetBool(PARAM_IS_REST, value);
    }

    public void EventFootStepSound()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.FootStepPlayerCharacter.ToString());
    }
}
