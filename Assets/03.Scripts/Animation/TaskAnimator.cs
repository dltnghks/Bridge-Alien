using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UITaskState
{
    // 기본 애니
    Idle,
    
    //  자기개발 (C01)
    C01_T01, //	헬스
    C01_T02, //	실내 클라이밍
    C01_T03, //	지구일보 구독
    C01_T04, //	지구사 연구
    
    // 운세관리 (C02)
    C02_T01, //	기도
    C02_T02, //	쓰레기 줍기
    
    // 휴식 & 유흥 (C03)
    C03_T01, //	가만히 있기
    C03_T02, //	노래방
    C03_T03, //	헌팅 포차
    C03_T04, //	병원 입원

}


[RequireComponent(typeof(Animator))]  
public class TaskAnimator : MonoBehaviour
{
    private Animator animator;                                                  
    private UITaskState currentState;   
    
    private void Awake()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }          // 애니메이터 컴포넌트 참조
        currentState = UITaskState.Idle;                                     // 초기 상태 설정
    }

    // 추가적인 애니메이션은 TaskAnimPortrait_Controller에서 Trigger 설정해줘야 됨.
    public void TriggerTask(string taskId)
    {   
        animator.Rebind();                     // 상태 초기화
        animator.ResetTrigger(taskId);    // 혹시 설정되어 있던 Trigger 제거
        animator.SetTrigger(taskId);      // Trigger 실행
    }
}
