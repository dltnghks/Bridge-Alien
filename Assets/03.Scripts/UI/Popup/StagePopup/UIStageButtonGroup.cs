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

        _uiStageButtonsList = GetComponentsInChildren<UIStageButton>();

        return true;
    }

    public void InitStageButtonGroup()
    {
        Init();

        foreach (Define.StageType stageType in System.Enum.GetValues(typeof(Define.StageType)))
        {
            // 버튼이 부족한 경우 중단
            if (_uiStageButtonsList.Length <= (int)stageType)
            {
                break;
            }

            var stageData = Managers.Data.StageData.GetStageData(stageType);
            UpdateStageButtonState((int)stageType, stageType);

            SetStageStars(stageType, stageData.IsLocked);

        }
    }

    // 스테이지 버튼 그룹의 상태를 업데이트
    private void UpdateStageButtonState(int index, Define.StageType stageType)
    {
        // 스테이지 잠금 여부 확인
        if (Managers.Stage.IsStageLockStatus(stageType))
        {
            _uiStageButtonsList[index].GetComponent<UIActiveButton>().Deactivate();
        }
        else
        {
            _uiStageButtonsList[index].GetComponent<UIActiveButton>().Activate();
        }
    }

    // 스테이지 버튼 그룹의 별 개수를 업데이트
    private void SetStageStars(Define.StageType stageType, bool isLocked)
    {
        int starCount = Managers.Player.GetStageClearInfo(stageType);
        _uiStageButtonsList[(int)stageType].SetStageButton(starCount);
    }

}