using System.Collections.Generic;
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

        _uiStageButtonsList = GetComponentsInChildren<UIStageButton>(true);

        return true;
    }

    public void InitStageButtonGroup()
    {
        Init();

        if (_uiStageButtonsList == null || _uiStageButtonsList.Length == 0)
        {
            _uiStageButtonsList = GetComponentsInChildren<UIStageButton>(true);
        }

        foreach (Define.ChapterType stageType in System.Enum.GetValues(typeof(Define.ChapterType)))
        {
            if (_uiStageButtonsList.Length <= (int)stageType)
            {
                break;
            }

            var stageData = Managers.Data.StageData.GetStageData(stageType);
            UpdateStageButtonState((int)stageType, stageType);
            SetStageStars(stageType, stageData.IsLocked);
        }
    }

    private void UpdateStageButtonState(int index, Define.ChapterType stageType)
    {
        if (Managers.Stage.IsStageLockStatus(stageType))
        {
            _uiStageButtonsList[index].GetComponent<UIActiveButton>().Deactivate();
        }
        else
        {
            _uiStageButtonsList[index].GetComponent<UIActiveButton>().Activate();
        }
    }

    private void SetStageStars(Define.ChapterType stageType, bool isLocked)
    {
        int starCount = Managers.Player.GetStageClearInfo(stageType);
        _uiStageButtonsList[(int)stageType].SetStageButton(starCount, 3);
    }
}
