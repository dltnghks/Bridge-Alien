using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private Transform player;
    private float followSpeed = 2f; // 보스 이동 속도
    private Vector3 offset = new Vector3(-10f, 5f, 0f); // 플레이어의 좌측 상단 위치 오프셋
    private Vector3 originalPos;

    public MiniGameDeliveryEnemy Enemy { get; set; }
    
    public IdleState(MiniGameDeliveryEnemy enemy)
    {
        Enemy = enemy;
        player = Enemy.TargetPlayer.transform;
    }


    public void EnterState()
    {
        Debug.Log("Enter Idle State");
    }


    private float curTime = 0;
    public void UpdateState()
    {
        // 타겟의 좌측 상단 위치 계산
        Vector3 targetPosition = player.position + offset;
        targetPosition.x = Enemy.InitPosition.x;
        
        // 보스를 타겟 위치로 부드럽게 이동
        Enemy.MoveToDestination(targetPosition, followSpeed);
        
        curTime += Time.deltaTime;
        if (curTime >= 3f)
        {
            Enemy.SetState(new ChargeState(Enemy));
            curTime = 0;
        }
    }

    public void ExitState()
    {
        Debug.Log("Exit Idle State");
    }
}
