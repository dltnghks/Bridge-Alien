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
                ClosePopupUI();
            });
        
        return true;
    }

    public override void Init(object data)
    {
        base.Init(data);
        
        if (data is PlayerTaskData taskData)
        {    
            // 결과 팝업 예약
            Managers.UI.RequestPopup<UITaskResultPopup>(data);
            
            // 애니메이션 설정
            _taskAnimator.TriggerTask(taskData.TaskID);

            // 텍스트 설정
            GetText((int)Texts.TaskProgressText).text = taskData.TaskProgressText;
        }
        else
        {
            Logger.LogWarning("data is not PlayerTaskData");
        }
    }
}
