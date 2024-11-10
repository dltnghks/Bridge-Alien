using System;
using UnityEngine.Events;

public interface ITimed
{
    bool IsActive { get; set; }
    UnityAction EndTimerAction { get; set; }
    float CurTime { get; set; }
    
    void SetTimer(float time,UnityAction endTimerAction);  // 타이머 시작 메서드
    void TimerUpdate();
    void AddTime(float time);         // 남은 시간 조회
    void EndTimer();
}