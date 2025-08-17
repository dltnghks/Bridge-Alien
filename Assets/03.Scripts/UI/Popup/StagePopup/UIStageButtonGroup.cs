using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

public class UIStageButtonGroup : UISubItem
{
    private UIStageButton[] _uiStageButtonsList;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _uiStageButtonsList = GetComponentsInChildren<UIStageButton>();

        return true;
    }

    public void InitStageButtonGroup()
    {
        Init();
        
        foreach (var stageButton in _uiStageButtonsList)
        {
            stageButton.InitStageButton();
        }
    }

}