using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager
{
    private Define.StageType _currentStageType;
    private StageData _currentStageData;

    public Action<StageData> OnChangeStage;

    public Define.StageType CurrentStageType => _currentStageType;

    public void Init()
    {
        _currentStageData = null;
    }

    public StageData GetCurrentStageData()
    {
        if (_currentStageData is null)
        {
            Logger.LogWarning("설정된 스테이지 데이터가 없습니다. 1-1 Stage를 로드합니다.");
            SetCurrentStage(Define.StageType.Stage1_1);
            return _currentStageData;
        }
        return _currentStageData;
    }

    public void SetCurrentStage(Define.StageType stageType)
    {
        var stageData = Managers.Data.StageData.GetStageData(stageType);

        if (stageData is null) return;

        _currentStageType = stageType;
        _currentStageData = stageData;
        OnChangeStage?.Invoke(_currentStageData);
    }

    // 스테이지 클리어 처리, 클리어 결과 별 반환
    public int CompleteStage(int playerScore)
    {
        Logger.Log($"Stage Complete! Score : {playerScore}");
        int starCount = 0;

        foreach (var score in _currentStageData.ClearScoreList)
        {
            if (playerScore >= score)
            {
                starCount++;
            }
        }

        return starCount;
    }

    public int GetCompleteTotalGold(int starCount)
    {
        int totalGold = _currentStageData.ClearReward * Math.Max(0, starCount - Managers.Player.GetStageClearInfo(_currentStageType));

        // 골드 계산 후 저장
        Managers.Player.SaveStageProgress(_currentStageType, starCount);

        return totalGold;
    }

    // 현재 스테이지를 진행할 수 있는가 확인
    public bool IsStageLockStatus(Define.StageType stageType)
    {
        var stage = Managers.Data.StageData.GetStageData(stageType);
        if (stage.IsLocked == false)
        {
            return false;
        }

        // 이전 스테이지를 클리어한 경우에만 true
        Logger.Log($"{(int)stageType}, {Managers.Player.GetCleardStageNum()} : {(int)stageType > Managers.Player.GetCleardStageNum()}");
        // 클리어한 스테이지 수가 현재 스테이지보다 작으면 잠금 상태
        if ((int)stageType > Managers.Player.GetCleardStageNum())
        {
            return true;
        }

        return false;
    }

    public string ToStageString(Define.StageType stageType)
    {
        return stageType.ToString().Replace("Stage", "").Replace('_', '-');
    }

}
