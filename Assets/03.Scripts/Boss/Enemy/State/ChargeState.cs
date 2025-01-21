using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChargeState : IEnemyState
{
    private enum EChargeState
    {
        Ready,      // 준비 자세
        Warning,    // 경고 표시
        Charging,   // 돌진
    }
    
    private float _warningDuration = 2f;       // 경고 표시 시간
    private float _curTime = 0f;                // 경고 시간
    private float _chargeSpeed = 10f;          // 돌진 속도
    private Vector3 _chargeStartPosition;           // 돌진 시작 위치
    private Vector3 _targetDirection;          // 돌진 방향
    
    private EChargeState _chargeState = EChargeState.Ready;           // true 돌진 중, false 준비 중
    
    public MiniGameDeliveryEnemy Enemy { get; set; }
    
    public ChargeState(MiniGameDeliveryEnemy enemy)
    {
        Enemy = enemy;
    }


    public void EnterState()
    {
        Debug.Log("Enter Charge State");
        
        _chargeState = EChargeState.Ready;
        
        Vector3 pos = Enemy.transform.position;
        pos.y = Enemy.TargetPlayer.transform.position.y;
        _chargeStartPosition = pos;
    }

    public void UpdateState()
    {
        // 돌진 중 동작 로직 (추적 등)
        if (_chargeState == EChargeState.Ready)
        {
            Enemy.MoveToDestination(_chargeStartPosition, _chargeSpeed, () =>
            {
                _chargeState = EChargeState.Warning;
                _chargeStartPosition.x += 40.0f;
                // 경고 영역 표시
                CreateWarningArea();
            });
        }
        else if (_chargeState == EChargeState.Warning)
        {
            _curTime += Time.deltaTime;
            if (_curTime >= _warningDuration)
            {
                _curTime = 0;
                _chargeState = EChargeState.Charging;
            }
        }
        else if(_chargeState == EChargeState.Charging)
        {
            Enemy.MoveToDestination(_chargeStartPosition, _chargeSpeed, ()=>
            {
                Enemy.SetState(new IdleState(Enemy));
            });
        }
    }
    
    public void ExitState()
    {
        Debug.Log("Exiting Charge State");

    }

    private void CreateWarningArea()
    {
        // 경고 영역 생성 및 설정
    }
    
}