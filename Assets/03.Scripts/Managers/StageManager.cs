using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class StageManager
{
    private Define.ChapterType _currentStageType;
    private StageData _currentStageData;
    private bool _isStageCleared = false;

    public Action<StageData> OnChangeStage;

    public Define.ChapterType CurrentStageType => _currentStageType;

    public void Init()
    {
        _currentStageData = null;
        _isStageCleared = false;
    }

    public StageData GetCurrentStageData()
    {
        if (_currentStageData is null)
        {
            Logger.LogWarning("설정된 스테이지 데이터가 없습니다. 1-1 Stage를 로드합니다.");
            SetCurrentStage(Define.ChapterType.CH1);
            return _currentStageData;
        }
        return _currentStageData;
    }

    public void SetCurrentStage(Define.ChapterType stageType)
    {
        var stageData = Managers.Data.StageData.GetStageData(stageType);

        if (stageData is null) return;

        _currentStageType = stageType;
        _currentStageData = stageData;
        OnChangeStage?.Invoke(_currentStageData);
    }

    public void StartStage()
    {
        Logger.Log("Stage Start");
        if (_currentStageData is null)
        {
            Logger.LogError("설정된 스테이지 데이터가 없습니다.");
            return;
        }

        // 클리어 여부 세팅
        _isStageCleared = false;
        if (Managers.Player.GetStageClearInfo(_currentStageType) > 0)
        {
            _isStageCleared = true;
        }

        // 스테이지 시작 시 이벤트 재생
        // 클리어했던 스테이지거나 이미 진행한 스테이지인 경우 바로 씬 변경
        if (Managers.Player.GetStageProgressedStatus(_currentStageType))
        {
            Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload);
        }
        else
        {
            Managers.Event.Init(_currentStageData.EventID);
        }
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

    public void EndStage()
    {
        if (_isStageCleared)
        {
            Managers.Scene.ChangeScene(Define.Scene.House);
        }
        else
        {
            Managers.Event.Init(_currentStageData.ClearEventID);
        }
    }

    public int GetCompleteTotalGold(int starCount)
    {
        int totalGold = _currentStageData.ClearReward * Math.Max(0, starCount - Managers.Player.GetStageClearInfo(_currentStageType));

        // 골드 계산 후 저장
        Managers.Player.SaveStageProgress(_currentStageType, starCount);

        return totalGold;
    }

    // 현재 스테이지를 진행할 수 있는가 확인
    public bool IsStageLockStatus(Define.ChapterType stageType)
    {
        var stage = Managers.Data.StageData.GetStageData(stageType);
        if (stage.IsLocked == false)
        {
            return false;
        }

        // 클리어한 스테이지 수가 현재 스테이지보다 작으면 잠금 상태
        if ((int)stageType > Managers.Player.GetCleardStageNum())
        {
            return true;
        }

        return false;
    }

    public string ToStageString(Define.ChapterType stageType)
    {
        return stageType.ToString();
    }

}
