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
    
    public void SetTimer(UITimer uiTimer, float time, UnityAction endTimerAction = null)
    {
        if (!IsActive)
        {
            UITimer = uiTimer;
            IsActive = true;
            StartTime = time;
            CurTime = time;
            EndTimerAction = endTimerAction;

            UITimer.SetTimer(time);
        }
    }

    public void ResetTimer()
    {
        CurTime = StartTime;
        UITimer.SetTimer(StartTime);
        IsActive = true;
        Debug.Log("Reset Timer");
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
            Debug.Log("End Timer");
        }
    }
}
