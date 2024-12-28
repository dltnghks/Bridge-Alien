using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerBase
{
    public UITimer UITimer { get; private set; }
    public bool IsActive { get; private set; }
    public UnityAction EndTimerAction { get; private set; }
    public float StartTime { get; private set; }
    public float CurTime { get; private set; }

    public void OffTimer()
    {
        IsActive = false;
    }
    
    public void SetTimer(UITimer uiTimer, float time, UnityAction endTimerAction = null)
    {
        if (!IsActive)
        {
            UITimer = uiTimer;
            StartTime = time;
            CurTime = time;
            EndTimerAction = endTimerAction;

            UITimer.SetTimer(StartTime, CurTime);
            UITimer.Init();
            IsActive = true;
        }
    }

    public void RestartTimer()
    {
        CurTime = StartTime;
        UITimer.SetTimer(StartTime);
        IsActive = true;
    }
    
    
    public void TimerUpdate()
    {
        float deltaTime = -Time.deltaTime;
        AddTime(deltaTime);
            
        if (CurTime <= 0 && IsActive)
        {
            EndTimer();
        }
    }

    public void AddTime(float time)
    {
        if (IsActive)
        {
            CurTime += time;
            UITimer.AddTime(time);
        }
    }

    public void EndTimer()
    {
        if (IsActive)
        {
            EndTimerAction?.Invoke();
            IsActive = false;
        }
    }
}
