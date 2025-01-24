using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRockState : IEnemyState
{
    private GridManager gridManager;                    // 그리드 매니저
    private float attackInterval = 2.5f;  // 공격 간격
    private float currentTime = 0f;
    
    public MiniGameDeliveryEnemy Enemy { get; set; }
    
    public DropRockState(MiniGameDeliveryEnemy enemy)
    {
        Enemy = enemy;
        gridManager = GameObject.FindObjectOfType<GridManager>();   // 그리드 매니저 찾기
    }

    public void EnterState()
    {
        Debug.Log("Enter DropRock State");
        ExecuteGridPattern(); // 상태 진입시 한 번만 실행
    }

    public void UpdateState()
    {
        // 그리드 패턴 실행 제거
    }

    public void ExitState()
    {
        Debug.Log("Exit DropRock State");
    }

    private void ExecuteGridPattern()
    {
        if (gridManager != null)
        {
            // 50% 확률로 위에서 아래로 또는 아래에서 위로 패턴 선택
            if (Random.value < 0.5f)
            {
                // 위에서 아래로
                gridManager.ExecuteTopToBottomPattern();
            }
            else
            {
                // 아래에서 위로
                gridManager.ExecuteBottomToTopPattern();
            }
        }
    }
}
