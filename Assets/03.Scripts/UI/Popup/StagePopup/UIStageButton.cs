using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageButton : UISubItem
{
    enum Texts
    {
        StageNameText
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
        Managers.Stage.LoadStage(_stageType);
    }
}

