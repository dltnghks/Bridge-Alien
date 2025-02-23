using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameDeliveryPathProgress
{
    public UIPathProgressBar UIPathProgressBar { get; private set; }
    public bool IsActive { get; private set; }
    public UnityAction EndAction { get; private set; }
    public float EndValue { get; private set; }
    public float CurValue { get; private set; }

    public void SetProgressBar(UIPathProgressBar uIPathProgressBar, float endTime, UnityAction endAction = null){
        if (!IsActive)
        {
            UIPathProgressBar = uIPathProgressBar;
            EndValue = endTime;
            CurValue = 0;
            EndAction = endAction;


            UIPathProgressBar.Init();
            UIPathProgressBar.SetProgressBar(endTime);
            
            IsActive = true;
        }
    }

    public void ProgressUpdate(){
        float deltaTime = Time.deltaTime;
        AddProgress(deltaTime);
            
        if (CurValue >= EndValue && IsActive)
        {
            EndProgress();
        }
    }
    
    public void AddProgress(float time)
    {
        if (IsActive)
        {
            CurValue += time;
            UIPathProgressBar.AddProgress(time);
        }
    }

    public void EndProgress()
    {
        Logger.Log("EndProgress");
        if (IsActive)
        {
            EndAction?.Invoke();
            IsActive = false;
        }
    }
}
