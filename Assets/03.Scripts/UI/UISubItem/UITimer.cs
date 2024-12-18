using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UITimer : UISubItem
{
    enum Texts
    {
        TimerText,        
    }

    enum Objects{
        TimerGauge,
    }
    
    [SerializeField] private bool _onGauge = false;
    [SerializeField] private bool _onTimerText = true;  

    private UIGauge _gauge;
    private float _curTime;
    private float _startTime;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindObject(typeof(Objects));

        _gauge = GetObject((int)Objects.TimerGauge).GetOrAddComponent<UIGauge>();

        if(!_onGauge){
            _gauge.gameObject.SetActive(false);
        }
        if(!_onTimerText){
            GetText((int)Texts.TimerText).gameObject.SetActive(false);
        }

        return true;
    }
    
    public void SetTimer(float startTime, float time)
    {
        Init();
        _startTime = startTime;
        _curTime = time;
        SetTimerText(time);
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

        _gauge.SetGauge(time/_startTime);
        //_timerSlider.value = time/_startTime;
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
