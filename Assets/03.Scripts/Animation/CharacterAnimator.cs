using UnityEngine;

public enum CharacterState
{
    // 기본 애니
    Idle,
    Walk,
    Run,
    
    // 게임 상태
    WinPose,
    LosePose,
}

[RequireComponent(typeof(Animator))]                                            // 애니메이터 컴포넌트 필요
public class CharacterAnimator : MonoBehaviour
{
    protected Animator animator;                                                  // 애니메이터 컴포넌트
    protected CharacterState currentState;                                        // 현재 상태 (enum)
    
    //~ 애니메이터 파라미터 이름 상수화
    protected static readonly string PARAM_IS_MOVING = "IsMoving";
    protected static readonly string PARAM_MOVE_SPEED = "MoveSpeed";
    protected static readonly string TRIGGER_WIN = "TriggerWin";
    protected static readonly string TRIGGER_LOSE = "TriggerLose";
    protected static readonly string TRIGGER_HOLD_UP = "TriggerHoldUp";
    protected static readonly string TRIGGER_HOLD_DOWN = "TriggerHoldDown";

    //~ 초기화
    protected void Awake()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }          // 애니메이터 컴포넌트 참조
        currentState = CharacterState.Idle;                                     // 초기 상태 설정
    }           

    //~ 기본 이동 상태 제어          
    public void UpdateMovement(float moveSpeed)         
    {           
        bool isMoving = moveSpeed > 0;                                                      // 이동 여부 확인
        animator.SetBool(PARAM_IS_MOVING, isMoving);                                        // 이동 여부 파라미터 설정
        animator.SetFloat(PARAM_MOVE_SPEED, moveSpeed);                                     // 이동 속도 파라미터 설정

        // moveSpeed에 따라 Walk/Run 상태 결정
        if (!isMoving) { ChangeState(CharacterState.Idle); }                                // 이동중 아니면 Idle 상태로 변경
        else { ChangeState(moveSpeed > 1f ? CharacterState.Run : CharacterState.Walk); }    // 이동중이면 속도에 따라 Run/Walk 상태로 변경
    }

    //~ 게임 결과 포즈
    public void PlayWinPose()
    {
        Logger.Log("PlayWinPose");
        animator.SetTrigger(TRIGGER_WIN);
        ChangeState(CharacterState.WinPose);
    }

    public void PlayLosePose()
    {
        Logger.Log("PlayLosePose");
        animator.SetTrigger(TRIGGER_LOSE);
        ChangeState(CharacterState.LosePose);
    }

    //~ 애니메이션 상태 변경 처리
    protected void ChangeState(CharacterState newState)
    {
        if (currentState == newState) return;
        
        // 이전 상태 종료 처리
        OnStateExit(currentState);
        
        // 새로운 상태 시작 처리
        currentState = newState;
        OnStateEnter(currentState);
    }

    //~ 애니메이션 상태 진입 처리
    protected void OnStateEnter(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.WinPose:
            case CharacterState.LosePose:
                // 결과 포즈 시 입력 차단 등의 처리
                break;
        }
    }

    //~ 애니메이션 상태 종료 처리
    protected void OnStateExit(CharacterState state)
    {
        switch (state)
        {
            case CharacterState.WinPose:
            case CharacterState.LosePose:
                // 결과 포즈 종료 시 처리
                break;
        }
    }
    
    public bool IsInResultPose() =>
        currentState == CharacterState.WinPose ||
        currentState == CharacterState.LosePose;

    public bool IsMoving() =>
        currentState == CharacterState.Walk || 
        currentState == CharacterState.Run;
}