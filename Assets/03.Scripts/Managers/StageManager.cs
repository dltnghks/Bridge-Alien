using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager
{
    private Define.StageType _currentStageType;
    private StageData _currentStage;

    public Action<StageData> OnChangeStage;

    public void Init()
    {
        _currentStage = null;
    }

    public void LoadStage(Define.StageType stageType)
    {
        var stageData = Managers.Data.StageData.GetStageData(stageType);

        if (stageData is null) return;

        _currentStageType = stageType;
        _currentStage = stageData;
        OnChangeStage?.Invoke(_currentStage);
    }

    // 스테이지 클리어 처리, 클리어 결과 별 반환
    public int CompleteStage(int playerScore)
    {
        int starCount = 0;

        foreach (var score in _currentStage.ClearScoreList)
        {
            if (playerScore >= score)
            {
                starCount++;
            }
        }

        Managers.Player.SaveStageProgress(_currentStageType, starCount);

        return starCount;
    }

    // 현재 스테이지를 진행할 수 있는가 확인
    public bool CheckStageLockStatus(Define.StageType stageType)
    {
        var stage = Managers.Data.StageData.GetStageData(stageType);
        if (stage.IsLocked == false)
        {
            return false;
        }

        if (stage.RequiredStars >= Managers.Player.GetTotalStars())
        {
            return true;
        }


        return false;
    }

}
