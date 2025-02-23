using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombState : IEnemyState
{
    private GridManager gridManager;                    // 그리드 매니저
    
    public MiniGameDeliveryEnemy Enemy { get; set; }
    
    public BombState(MiniGameDeliveryEnemy enemy)
    {
        Enemy = enemy;
        gridManager = GameObject.FindObjectOfType<GridManager>();   // 그리드 매니저 찾기
    }
    
    public void EnterState()
    {
        Debug.Log("Enter Bomb State");
        ExecuteGridPattern(); // 상태 진입시 한 번만 실행
    }

    public void UpdateState()
    {
        // 그리드 패턴 실행 제거
    }

    public void ExitState()
    {
        Debug.Log("Exit Bomb State");
    }

    private void ExecuteGridPattern()
    {
        if (gridManager != null)
        {
            // 폭탄 상태에서는 랜덤 패턴 사용
            gridManager.ExecuteRandomPattern();
        }
    }
}
