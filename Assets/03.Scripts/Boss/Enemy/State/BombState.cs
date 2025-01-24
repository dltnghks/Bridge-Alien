using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombState : MonoBehaviour // 이거 왜 IEnemyState 안쓰는거임? 딴 State들은 상속받던데?
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
        if (gridManager != null) { gridManager.ExecuteRandomPattern(); } // 랜덤 패턴 실행
    }

    public void UpdateState()
    {
        // 돌진 중 동작 로직 (추적 등)
    }

    public void ExitState()
    {
        Debug.Log("Exit Charge State");
    }
}
