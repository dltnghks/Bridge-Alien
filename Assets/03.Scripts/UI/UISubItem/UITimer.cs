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
        Background,
    }
    
    [SerializeField] private bool _onGauge = false;
    [SerializeField] private bool _onTimerText = true;  

    private UIGauge _gauge;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindObject(typeof(Objects));

        _gauge = GetObject((int)Objects.TimerGauge).GetOrAddComponent<UIGauge>();
        _gauge.Init();
        
        if(!_onGauge){
            _gauge.gameObject.SetActive(false);
        }
        if(!_onTimerText){
            GetText((int)Texts.TimerText).gameObject.SetActive(false);
            GetObject((int)Objects.Background).gameObject.SetActive(false);
        }

        return true;
    }

    public void SetTimerText(float time, float startTime)
    {
        Init();
        string timeFormat = GetTimeFormat(time);
        GetText((int)Texts.TimerText).SetText(timeFormat);

        _gauge.SetGauge(time / startTime);
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
