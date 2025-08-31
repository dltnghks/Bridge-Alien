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
        if(Managers.Player.PlayerData.TotalStars < Managers.Data.StageData.GetStageData(_stageType).RequiredStars)
        {
            // 해금 조건이 안된 경우
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

