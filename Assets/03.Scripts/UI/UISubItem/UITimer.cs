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
    
    private float _curTime;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));

        return true;
    }


    public void SetTimer(float time)
    {
        Init();
        _curTime = time;
        SetTimerText(time);
    }

    public void AddTime(float time)
    {
        _curTime += time;
        SetTimerText(_curTime);
    }
    

    private void SetTimerText(float time)
    {
        string timeFormat = GetTimeFormat(time);
        GetText((int)Texts.TimerText).SetText(timeFormat);
    }

    private string GetTimeFormat(float time)
    {
        if (time <= 0)
        {
            return "00:00";    
        }
        
        return $"{(int)time % 60:00}:{(time * 100) % 100:00}";
    }
}
