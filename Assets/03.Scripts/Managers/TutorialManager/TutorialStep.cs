using UnityEngine;

// 모든 튜토리얼 단계의 기본형
public abstract class TutorialStep : MonoBehaviour
{
    [Header("UI Setting")]
    public string instructionText; // 3. UI 텍스트 출력용

    // 단계 시작 시 1회 실행 (초기화, 상자 생성 등)
    public virtual void OnEnter() { }

    // 매 프레임 실행 (조건 체크: 상자가 레일에 도착했는가?)
    public virtual void OnExecute() { }

    // 단계 종료 시 1회 실행 (뒷정리)
    public virtual void OnExit() { }
    
    // 단계가 끝났는지 체크하는 플래그
    public bool IsFinished { get; protected set; }
}