using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageButton : UISubItem
{
    enum Texts
    {
        StageText
    }

    [SerializeField]
    private Define.StageType _stageType;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));

        gameObject.BindEvent(OnClickButton);

        return true;
    }
    
    private void OnClickButton()
    {
        Managers.Stage.SetCurrentStage(_stageType);
    }

    public void InitStageButton()
    {
        // 잠기지 않은 경우만 열어두기
        if (Managers.Stage.CheckStageLockStatus(_stageType) == false)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
}

