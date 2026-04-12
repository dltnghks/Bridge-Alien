using System;

public class StageManager
{
    private Define.ChapterType _currentStageType;
    private StageData _currentStageData;
    private bool _isStageCleared = false;
    private bool _isStageStarted = false;
    private int _endingThumbnailIndex = -1;
    private Define.EventDataID _endingBranchEventID = Define.EventDataID.Unknown;

    public Action<StageData> OnChangeStage;

    public Define.ChapterType CurrentStageType => _currentStageType;

    public void Init()
    {
        _currentStageData = null;
        _isStageCleared = false;
        _endingThumbnailIndex = -1;
        _endingBranchEventID = Define.EventDataID.Unknown;
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

        _endingThumbnailIndex = -1;
        _endingBranchEventID = Define.EventDataID.Unknown;

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

        if (_isStageStarted)
        {
            Logger.LogWarning("이미 스테이지가 시작되었습니다.");
            return;
        }
        _isStageStarted = true;

        int fatigue = Managers.Player.GetStats(Define.PlayerStatsType.Fatigue);
        if (fatigue <= 0)
        {
            Logger.Log("피로도가 부족하여 스테이지를 시작할 수 없습니다.");
            _isStageStarted = false;
            return;
        }

        // 클리어 여부 세팅
        _isStageCleared = false;
        if (Managers.Player.GetStageClearInfo(_currentStageType) > 0)
        {
            _isStageCleared = true;
        }

        // 스테이지 시작 시 피로도 차감
        bool isProgressed = Managers.Player.GetStageProgressedStatus(_currentStageType);
        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, -1);
        Managers.Player.SaveStageProgressed(_currentStageType);

        Managers.Analytics.TrackStageEnter(_currentStageType.ToString());

        // 스테이지 시작 시 이벤트 재생
        // 클리어했던 스테이지거나 이미 진행한 스테이지인 경우 바로 씬 변경
        if (isProgressed)
        {
            Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload);
        }
        else
        {
            Managers.Event.Init(_currentStageData.EventID);
        }
    }

    // 스테이지 클리어 처리, 클리어 결과 별 반환
    public int CompleteStage(int playerScore, int preStarCount)
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

        // 새로 추가된 별 개수만큼 피로도 회복
        Managers.Player.AddStats(Define.PlayerStatsType.Experience, (starCount - preStarCount) * 5);

        return starCount;
    }

    public void EndStage(int starCount)
    {
        _isStageStarted = false;

        if (_currentStageType == Define.ChapterType.End && starCount > 0 && _endingBranchEventID != Define.EventDataID.Unknown)
        {
            if (_endingThumbnailIndex >= 0)
            {
                Managers.Player.SaveEndingThumbnailProgress(_currentStageType, _endingThumbnailIndex);
            }

            Managers.Event.Init(_endingBranchEventID);
            _endingThumbnailIndex = -1;
            _endingBranchEventID = Define.EventDataID.Unknown;
            return;
        }

        // 이미 클리어했거나 별 개수가 0인 경우(챕터 클리어 실패)에는 바로 집으로 이동 
        if (_isStageCleared || starCount == 0)
        {
            Managers.Scene.ChangeScene(Define.Scene.House);
        }
        else
        {
            // 최초 클리어 시 보상 지급
            Managers.Player.AddStats(Define.PlayerStatsType.GravityAdaptation, 5);
            Managers.Player.AddStats(Define.PlayerStatsType.Experience, 5);
            Managers.Player.FillFatigue();
            Managers.Event.Init(_currentStageData.ClearEventID);
        }
    }

    public void SetEndingResult(Define.EventDataID eventID, int thumbnailIndex)
    {
        _endingBranchEventID = eventID;
        _endingThumbnailIndex = thumbnailIndex;
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

