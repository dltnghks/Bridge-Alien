using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPathProgressBar : UISubItem
{
    enum Objects
    {
        PathProgressBar,
    }

    private Slider _progressBar;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));

        _progressBar = GetObject((int)Objects.PathProgressBar).GetComponent<Slider>();
        Initialized();

        _init = true;

        return true;
    }

    private void Initialized()
    {
        _progressBar.value = 0;
    }

    public void SetProgressBar(float currValue, float endValue)
    {
        _progressBar.value = currValue;
        _progressBar.maxValue = endValue;
    }

    public void UpdateProgress(float value)
    {
        _progressBar.value = value;
    }
}
