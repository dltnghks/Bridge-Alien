using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryEnemy : MonoBehaviour
{
    private IEnemyState _currentState;
    
    public Player TargetPlayer;

    public Vector3 InitPosition { get; private set; }
    
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        InitPosition = transform.position;
        SetState(new IdleState(this));
    }
    
    public void SetState(IEnemyState newState)
    {
        Logger.Log($"SetState: {newState.GetType().Name}");
        _currentState?.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    private void Update()
    {
        if (Managers.MiniGame.CurrentGame.IsPause)
            return;
        _currentState?.UpdateState();
    }

    public void MoveToDestination(Vector3 destination, float speed, Action onArrival = null)
    {
        if (Managers.MiniGame.CurrentGame.IsPause)
            return;

        // 목적지와 현재 위치의 거리 계산
        float distance = Vector3.Distance(transform.position, destination);

        // 보스를 목적지로 이동
        transform.position = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);

        // 도착 여부 확인 (거리 기준)
        if (distance <= 0.1f)
        {
            // 도착 후 콜백 실행
            onArrival?.Invoke();
        }
    }
}
