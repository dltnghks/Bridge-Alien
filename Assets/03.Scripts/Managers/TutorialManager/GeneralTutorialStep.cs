using UnityEngine;
using UnityEngine.Events;

public class GeneralTutorialStep : TutorialStep // 아까 만든 부모 클래스 상속
{
    [Header("설정")]
    public string targetInteractableTag; // 이 단계에서 허용할 상호작용 태그 (요구사항 2)
    public float autoFinishDelay = 0f;   // 0보다 크면 시간 지나고 자동 종료 (요구사항 예시 6번)

    [Header("이벤트 연결")]
    public UnityEvent onStepStart; // 시작할 때 실행할 함수들 (상자 생성 등)
    public UnityEvent onStepExecute; // 매 프레임 실행할 것들
    public UnityEvent onStepExit;  // 끝날 때 실행할 함수들

    // 완료 조건 (예: 특정 불리언 변수가 참이 되면 끝남)
    // 복잡한 조건은 별도 컴포넌트로 빼거나 여기서 단순화 가능
    public bool isConditionMet = false; 

    public override void OnEnter()
    {
        // 1. 매니저에 허용 태그 설정
        if (TutorialManager.Instance != null)
            TutorialManager.Instance.AllowedInteractableTag = targetInteractableTag;

        // 2. 이벤트 실행 (예: 상자 생성 함수 호출)
        onStepStart?.Invoke();
    }

    public override void OnExecute()
    {
        onStepExecute?.Invoke();

        // 자동 종료 타이머가 있다면 처리
        if (autoFinishDelay > 0)
        {
            autoFinishDelay -= Time.deltaTime;
            if (autoFinishDelay <= 0) IsFinished = true;
        }

        // 외부에서 조건을 충족시켰다면 종료 (예: 상자가 도착해서 isConditionMet을 true로 바꿈)
        if (isConditionMet)
        {
            IsFinished = true;
        }
    }

    public override void OnExit()
    {
        onStepExit?.Invoke();
    }
    
    // 외부(상자 도착 이벤트 등)에서 이 함수를 호출해 단계를 끝냄
    public void CompleteStep()
    {
        isConditionMet = true;
    }
}