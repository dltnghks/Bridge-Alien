using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPathProgressBar : UISubItem
{
    enum Objects{
        PathProgressBar,
    }

    private Slider _progressBar;

    private float _endValue = 0;
    private float _curValue = 0;

    public override bool Init()
    {
        if(base.Init() == false){
            return false;
        }

        BindObject(typeof(Objects));

        _progressBar = GetObject((int)Objects.PathProgressBar).GetComponent<Slider>();
        _progressBar.value = 0;

        _init = true;

        return true;
    }

    public void SetProgressBar(float endValue){
        _endValue = endValue;
        _progressBar.maxValue = endValue;
        _curValue = 0;
        _progressBar.minValue = 0;
    }

    public void AddProgress(float value){
        _curValue += value;
        _progressBar.value += value;
    }


}
