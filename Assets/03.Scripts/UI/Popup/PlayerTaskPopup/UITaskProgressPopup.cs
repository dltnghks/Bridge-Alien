using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITaskProgressPopup : UIPopup
{
    enum Texts
    {
        TaskProgressText,
    }

    enum Objects
    {
        AnimationImage,
        UIProgressBar,
    }
    
    private TaskAnimator _taskAnimator;
    private Slider _slider;
    private PlayerTaskExecutionData _executionData;
    private PlayerTaskData _taskData;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        BindObject(typeof(Objects));
        
        _taskAnimator = GetObject((int)Objects.AnimationImage).GetComponent<TaskAnimator>();
        _slider = GetObject((int)Objects.UIProgressBar).GetComponent<Slider>();
        
        _slider.value = 0;
        _slider.DOValue(1.0f, 5f).OnComplete(
            () =>
            {
                CompleteTask();
                ClosePopupUI();
            });
        
        return true;
    }

    public override void Init(object data)
    {
        base.Init(data);
        
        if (data is PlayerTaskExecutionData executionData)
        {
            _executionData = executionData;
            _taskData = executionData.TaskData;
            SetTaskData(_taskData);
        }
        else if (data is PlayerTaskData taskData)
        {
            _taskData = taskData;
            SetTaskData(_taskData);
        }
        else
        {
            Logger.LogWarning("data is not PlayerTaskData");
        }
    }

    private void SetTaskData(PlayerTaskData taskData)
    {
        if (taskData == null)
        {
            return;
        }

        // 애니메이션 설정
        _taskAnimator.TriggerTask(taskData.TaskID);

        // 텍스트 설정
        GetText((int)Texts.TaskProgressText).text = taskData.TaskProgressText;
    }

    private void CompleteTask()
    {
        _executionData?.Apply();

        if (_executionData != null)
        {
            Managers.UI.RequestPopup<UITaskResultPopup>(_executionData);
        }
        else if (_taskData != null)
        {
            Managers.UI.RequestPopup<UITaskResultPopup>(_taskData);
        }
    }

    public override void ClosePopupUI()
    {
        Managers.UI.SetInputBackground(true);
        base.ClosePopupUI();
    }
}
