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

        foreach (Define.StageType stageType in System.Enum.GetValues(typeof(Define.StageType)))
        {
            // 버튼이 부족한 경우 중단
            if (_uiStageButtonsList.Length <= (int)stageType)
            {
                break;
            }

            var stageData = Managers.Data.StageData.GetStageData(stageType);
            UpdateStageButtonState((int)stageType, stageData.RequiredStars);

            SetStageStars(stageType, stageData.IsLocked);

        }
    }

    // 스테이지 버튼 그룹의 상태를 업데이트
    private void UpdateStageButtonState(int index, int requiredStars)
    {
        // 별이 부족한 경우, 비활성화
        if (Managers.Player.PlayerData.TotalStars < requiredStars)
        {
            _uiStageButtonsList[(int)index].GetComponent<UIActiveButton>().Deactivate();
        }
        // 별이 충분한 경우, 활성화
        else
        {
            _uiStageButtonsList[(int)index].GetComponent<UIActiveButton>().Activate();
        }
    }

    // 스테이지 버튼 그룹의 별 개수를 업데이트
    private void SetStageStars(Define.StageType stageType, bool isLocked)
    {
        if (Managers.Player.PlayerData.ClearedStages.ContainsKey(stageType) || isLocked == false)
        {
            Logger.Log($"{stageType} : {Managers.Player.PlayerData.ClearedStages[stageType]}");
            _uiStageButtonsList[(int)stageType].SetStageButton(Managers.Player.PlayerData.ClearedStages[stageType]);
        }
        else
        {
            // 클리어 기록이 없는 경우
            _uiStageButtonsList[(int)stageType].SetStageButton(0);
        }
    }

}