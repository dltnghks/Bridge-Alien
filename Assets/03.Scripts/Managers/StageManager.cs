using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager
{
    private StageData _currentStage;

    public Action<StageData> OnChangeStage;

    public void Init()
    {
        _currentStage = null;
    }

    public void SetCurrentStage(StageData stageData)
    {
        if (stageData is null) return;

        _currentStage = stageData;
        OnChangeStage?.Invoke(_currentStage);
    }

    public void LoadStage(Define.StageType stageType)
    {
        var stageData = Managers.Data.StageData.GetStageData(stageType);
        SetCurrentStage(stageData);
    }

    // 스테이지 클리어 처리, 클리어 결과 별 반환
    public int CompleteStage(Define.StageType stageType, int playerScore)
    {
        int starCount = 0;

        foreach (var score in _currentStage.ClearScoreList)
        {
            if (playerScore >= score)
            {
                starCount++;
            }
        }

        Managers.Player.SaveStageProgress(stageType, starCount);

        return starCount;
    }

    // 현재 스테이지를 진행할 수 있는가 확인
    public bool CheckStageLockStatus(Define.StageType stageType)
    {
        if (_currentStage.IsLocked == true)
        {
            return true;
        }

        if (_currentStage.RequiredStars >= Managers.Player.GetTotalStars())
        {
            return true;
        }


        return false;
    }

}
