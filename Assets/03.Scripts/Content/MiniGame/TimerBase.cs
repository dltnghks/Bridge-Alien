using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerBase
{
    public bool IsActive { get; private set; }
    public float StartTime { get; private set; }
    public float CurTime { get; private set; }

    public Action<float, float> OnChangedTime;
    public UnityAction OnEndTime;

    public void OffTimer()
    {
        IsActive = false;
    }
    
    public void SetTimer(float time)
    {
        if (!IsActive)
        {
            Logger.Log("Set timer");
            StartTime = time;
            CurTime = time;

            IsActive = true;
        }
    }

    public void RestartTimer()
    {
        CurTime = StartTime;

        OnChangedTime?.Invoke(CurTime, StartTime);

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

            OnChangedTime?.Invoke(CurTime, StartTime);
        }
    }

    public void EndTimer()
    {
        if (IsActive)
        {
            OnEndTime?.Invoke();
            OnChangedTime?.Invoke(CurTime, StartTime);
            IsActive = false;
        }
    }
}
