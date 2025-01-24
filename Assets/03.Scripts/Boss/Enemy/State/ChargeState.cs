using System.Collections;
using System.Collections.Generic;
//using DG.Tweening;
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
    
    private GridManager gridManager;            // 그리드 매니저
    
    public MiniGameDeliveryEnemy Enemy { get; set; }
    
    public ChargeState(MiniGameDeliveryEnemy enemy)
    {
        Enemy = enemy;
        gridManager = GameObject.FindObjectOfType<GridManager>();   // 그리드 매니저 찾기
    }

    public void EnterState()
    {
        Debug.Log("Enter Charge State");
        if (gridManager != null)
        {
            gridManager.ExecuteLeftToRightPattern(); // 왼쪽에서 오른쪽으로 그리드 프리팹 생성
        }
        
        _chargeState = EChargeState.Ready;
        
        Vector3 pos = Enemy.transform.position;
        pos.y = Enemy.TargetPlayer.transform.position.y;
        _chargeStartPosition = pos;
    }

    public void UpdateState()
    {
        // 테스트용 키 입력 처리 추가
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _chargeState = EChargeState.Ready;
            Debug.Log("Ready 상태로 변경");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _chargeState = EChargeState.Warning;
            Debug.Log("Warning 상태로 변경");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _chargeState = EChargeState.Charging;
            Debug.Log("Charging 상태로 변경");
        }

        // 기존 상태 업데이트 로직
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