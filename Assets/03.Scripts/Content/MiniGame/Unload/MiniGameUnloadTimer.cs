using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnloadTimer : ITimed
{
    public UITimer UITimer { get; set; }
    public bool IsActive { get; set; }
    public UnityAction EndTimerAction { get; set; }
    public float CurTime { get; set; }
 
    
    
    public void SetTimer(UITimer uiTimer, float time, UnityAction endTimerAction = null)
    {
        if (!IsActive)
        {
            UITimer = uiTimer;
            IsActive = true;
            CurTime = time;
            EndTimerAction = endTimerAction;

            UITimer.SetTimer(time);
        }
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
