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

    enum Objects
    {
        StarGroup,
    }

    [SerializeField]
    private Define.StageType _stageType;
    private UIStageStarGroup _starGroup;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindObject(typeof(Objects));

        _starGroup = GetObject((int)Objects.StarGroup).GetOrAddComponent<UIStageStarGroup>();

        gameObject.BindEvent(OnClickButton);

        return true;
    }
    
    private void OnClickButton()
    {
        // 잠긴 경우 상호작용 X
        if (Managers.Stage.IsStageLockStatus(_stageType))
        {
            return;
        }
        Managers.Stage.SetCurrentStage(_stageType);
    }

    public void SetStageButton(int starCount)
    {
        Init();

        string stageText = Managers.Stage.ToStageString(_stageType);
        GetText((int)Texts.StageText).SetText(stageText);
        _starGroup.SetStarCount(starCount);
    }
}

