using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UITimer : UISubItem
{
    enum Texts
    {
        TimerText,        
    }

    private bool _active;
    private float _curTime;
    private UnityAction _callback; 
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));

        _active = false;
        
        return true;
    }

    public void Update()
    {
        if (_active)
        {
            _curTime -= Time.deltaTime;
            SetTimerText(_curTime);
            if (_curTime <= 0.0f)
            {
                _callback?.Invoke();
                _active = false;
            }
        }
    }

    public void SetTimer(float time, UnityAction callback = null)
    {
        SetTimerText(time);
        _callback = callback;
        _curTime = time;
        _active = true;
    }

    private void SetTimerText(float time)
    {
        string timeFormat = GetTimeFormat(time);
        GetText((int)Texts.TimerText).SetText(timeFormat);
    }

    private string GetTimeFormat(float time)
    {
        return $"{(int)time / 60:00}:{(int)time % 60:00}";
    }
}
