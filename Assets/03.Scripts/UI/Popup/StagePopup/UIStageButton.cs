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

    enum Images
    {
        StageClearIcon,
    }

    [SerializeField]
    private Define.ChapterType _stageType;
    [SerializeField] private bool _useStarGroup = true;
    [SerializeField] private bool _useStageClearIcon = true;
    private UIStageStarGroup _starGroup;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        if (_useStarGroup)
        {
            BindObject(typeof(Objects));
        }
        if (_useStageClearIcon)
        {
            BindImage(typeof(Images));
        }

        if (_useStarGroup && GetObject((int)Objects.StarGroup) != null)
        {
            _starGroup = GetObject((int)Objects.StarGroup).GetOrAddComponent<UIStageStarGroup>();
        }

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
        if (_useStarGroup && _starGroup != null)
        {
            _starGroup.SetStarCount(starCount);
        }
        SetStageClearIcon(starCount);
    }

    public void SetStageClearIcon(int starCount)
    {
        if (_useStageClearIcon == false)
        {
            return;
        }

        Image stageClearIcon = GetImage((int)Images.StageClearIcon);
        if (stageClearIcon == null)
        {
            return;
        }

        stageClearIcon.color = Color.clear;
        if (starCount > 0)
        {
            stageClearIcon.color = Color.white;
        }
    }
}

