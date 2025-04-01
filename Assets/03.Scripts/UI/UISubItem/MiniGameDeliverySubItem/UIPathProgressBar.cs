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
        if(base.Init() == false){
            return false;
        }

        BindObject(typeof(Objects));

        _progressBar = GetObject((int)Objects.PathProgressBar).GetComponent<Slider>();
        _progressBar.value = 0;

        return _init = true;
    }

    public void UpdateProgress(float value)
    {
        _progressBar.value = value;
    }
}
