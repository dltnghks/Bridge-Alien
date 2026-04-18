using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnload : MonoBehaviour, IMiniGame
{
    [Header("Game Setting")]
    [SerializeField] private MiniGameUnloadSetting _gameSetting;
    [SerializeField] private SkillBase[] _skillList; // 스킬 리스트

    [Header("Delivery Point")]
    [SerializeField] private List<MiniGameUnloadDeliveryPoint> _deliveryPointList = new List<MiniGameUnloadDeliveryPoint>();
    [SerializeField] private MiniGameUnloadCoolingPoint _coolingPoint;

    [Header("Box Spawn Point")]
    [SerializeField] private List<MiniGameUnloadBoxSpawnPoint> _boxSpawnPoint;                   // 박스 생성 주기
    [Header("Ending Hidden Box Spawn Points")]
    [SerializeField] private List<MiniGameUnloadBoxSpawnPoint> _endingHiddenBoxSpawnPoints = new List<MiniGameUnloadBoxSpawnPoint>();

    [Header("Return Point")]
    [SerializeField] private MiniGameUnloadReturnPoint _returnPoint;

    [Header("Disposal Point")]
    [SerializeField] private MiniGameUnloadDisposalPoint _disposePoint;
    [Header("Hidden Box (Boss Stage)")]
    [SerializeField] private int _hiddenBoxSpawnInterval = 8;

    private ComboSystem _comboSystem;
    private const int HiddenBoxTargetCount = 2;
    private int _totalSpawnedBoxCount;
    private int _endingHiddenSpawnPointBoxCount;
    private int _hiddenBoxReservedCount;
    private int _hiddenBoxSpawnedCount;
    private int _hiddenBoxDisposedCount;

    public bool IsActive { get; set; }
    public bool IsPause { get; set; }
    public bool IsTutorialActive {get; set;}

    public Player PlayerCharacter { get; set; }
    public IPlayerController PlayerController { get; set; }

    public UIScene GameUI { get; set; }
    private UIGameUnloadScene _uiGameUnloadScene;

    private TimerBase _timer;
    private ScoreBase _score;
    private float _gameStartTime;

    public void InitializeUI()
    {
        Logger.Log("InitializeUI Starting game");

        _uiGameUnloadScene = Managers.UI.ShowSceneUI<UIGameUnloadScene>();
        GameUI = _uiGameUnloadScene;
        GameUI.Init();
    }

    private void Update()
    {
        if (!Managers.MiniGame.IsAcitveObject)
        {
            return;
        }

        if(_timer != null) _timer.TimerUpdate();
    }

    public void Initialize()
    {
        IsActive = false;
        IsPause = false;
        IsTutorialActive = false;
        _gameStartTime = 0f;
        _totalSpawnedBoxCount = 0;
        _endingHiddenSpawnPointBoxCount = 0;
        _hiddenBoxReservedCount = 0;
        _hiddenBoxSpawnedCount = 0;
        _hiddenBoxDisposedCount = 0;

        SetGameInfo();

        SetReturnPoint();

        SetDeliveryPointList();

        SetSpawnBoxList();

        SetColdArea();
        SetDisposalPoint();

        SetPlayerCharacter();

        SetComboSystem();

        // 다른 설정이 끝난 후 UI 설정
        SetGameUI();

        // 카메라 타겟으로 플레이어 캐릭터 설정
        if (PlayerCharacter != null)
        {
            Managers.Camera.Initialize(PlayerCharacter.transform);
        }
    }

    public void StartGame()
    {
        Logger.Log("UnloadGame Starting game");
        _gameStartTime = UnityEngine.Time.realtimeSinceStartup;

        // 게임 활성화
        IsActive = true;
        ResumeGame();
        
        // 스테이지를 처음 진행하는 경우 튜토리얼 진행
        if (TutorialManager.Instance != null && Managers.MiniGame.MiniGameTutorial[(int)Managers.Stage.CurrentStageType] == false){
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        IsTutorialActive = true;
        Managers.UI.ClosePopupUI();
        Managers.MiniGame.MiniGameTutorial[(int)Managers.Stage.CurrentStageType] = true;

        TutorialManager.Instance.StartTutorial();
    }

    private void SetGameInfo()
    {
        // 원래는 so로 하나의 세팅값만 필요했는데, 스테이지로 변경하면서 각 프리팹에 세팅하도록 변경
        //_gameSetting = Managers.Data.MiniGameData.GetMiniGameSettings<MiniGameUnloadSetting>(Define.MiniGameType.Unload);

        if (_gameSetting == null)
        {
            Logger.LogError("Not Found Game Information");
        }
    }

    private void SetColdArea()
    {
        if (_coolingPoint == null)
        {
            Logger.LogWarning("CoolingPoint is not set. The cooling area feature will be disabled.");
            return;
        }

        _coolingPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
        _coolingPoint.SetColdArea(_gameSetting.MaxColdAreaBoxIndex);
    }

    private void SetDeliveryPointList()
    {
        if (_deliveryPointList == null || _deliveryPointList.Count == 0)
        {
            Logger.LogWarning("DeliveryPointList is not set or empty. The delivery feature will be disabled.");
            return;
        }
        
        // DeliveryPoint 초기화
        foreach (var deliveryPoint in _deliveryPointList)
        {
            if (deliveryPoint == null)
            {
                Logger.LogWarning("Null deliveryPoint encountered in the list!");
                continue;
            }

            deliveryPoint.OnScoreAction += AddScore;
            deliveryPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
            if (_returnPoint != null)
            {
                deliveryPoint.OnReturnAction += _returnPoint.PlaceBox;
            }
            deliveryPoint.SetAction();
        }
    }

    private void SetDisposalPoint()
    {
        if (_disposePoint == null)
        {
            Logger.LogWarning("DisposePoint is not set. The disposal feature will be disabled.");
            return;
        }
        _disposePoint.OnScoreAction += AddScore;
        _disposePoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
        _disposePoint.OnDisposedAction += OnDisposedBox;
    }

    private void SetSpawnBoxList()
    {
        if (_boxSpawnPoint == null)
        {
            Logger.LogWarning("BoxSpawnPoint is not set. The box spawning feature will be disabled.");
            return;
        }

        foreach (var boxSpawnPoint in _boxSpawnPoint)
        {
            if (boxSpawnPoint == null)
            {
                Logger.LogWarning("Null boxSpawnPoint encountered in the list!");
                continue;
            }

            boxSpawnPoint.OnScoreAction += AddScore;
            boxSpawnPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
            boxSpawnPoint.SetBoxSpawnPoint();
        }
    }

    private void SetReturnPoint()
    {
        if (_returnPoint == null)
        {
            Logger.LogWarning("ReturnPoint is not set. The return feature will be disabled.");
            return;
        }

        _returnPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
        _returnPoint.OnScoreAction += AddScore;
    }

    private void SetPlayerCharacter()
    {
        PlayerCharacter = GameObject.Find("Player")?.GetComponent<Player>();
        if (PlayerCharacter == null)
        {
            Logger.LogError("PlayerCharacter not found or does not have a Player component!");
            return;
        }

        PlayerController = new MiniGameUnloadPlayerController(PlayerCharacter, _gameSetting.DetectionBoxRadius, _gameSetting.MoveSpeedReductionRatio, _uiGameUnloadScene.UIBoxPreview.UpdateUI);
        if (PlayerController == null)
        {
            Logger.LogError("Failed to initialize PlayerController!");
            return;
        }
        PlayerController.Init(PlayerCharacter);

        // 스킬 세팅
        if (PlayerController is ISkillController skillController)
        {
            // 스킬 UI
            _uiGameUnloadScene.UIPlayerInput.SetSkillInfo(_skillList);
            _uiGameUnloadScene.UIPlayerInput.SetSkillAction(skillController.OnSkill);

            skillController.SetSkillList(_skillList);
        }
        else
        {
            Logger.LogError("PlayerController does not implement ISkillController!");
        }
    }

    // 게임 설정과 박스 스폰 리스트가 설정이 되어있을 때, UI 세팅
    private void SetGameUI()
    {
        // UI 초기화
        _timer = new TimerBase();
        _score = new ScoreBase();

        _score.OnChangedScore += _uiGameUnloadScene.UIScoreBoard.SetScore;
        _timer.OnChangedTime += _uiGameUnloadScene.UITimer.SetTimerText;
        _timer.OnEndTime += EndGame;

        _timer.SetTimer(_gameSetting.GamePlayTime);

        _comboSystem = new ComboSystem();
        if (_comboSystem != null && _uiGameUnloadScene.UIComboDisplay != null)
        {
            _comboSystem.OnComboChanged += _uiGameUnloadScene.UIComboDisplay.UpdateCombo;
            _comboSystem.OnComboBroken += _uiGameUnloadScene.UIComboDisplay.BreakCombo;
            _comboSystem.OnChangedComboBox += _uiGameUnloadScene.UIComboBoxView.UpdateUI;
            _comboSystem.OnComboBoxFull += HandleComboBoxFull; // 이벤트 구독
        }
    }

    private void SetComboSystem()
    {
        _comboSystem = new ComboSystem();
    }

    private void HandleComboBoxFull()
    {
        List<MiniGameUnloadBox> comboSnapshot = new List<MiniGameUnloadBox>(_comboSystem._comboBoxList.BoxList);

        // 현재 콤보 결과만 스냅샷으로 보존하고, 실제 스택은 즉시 비워 다음 입력을 받는다.
        _comboSystem.ClearComboBoxList();

        bool isSpecialCombo = CheckForSpecialCombo(comboSnapshot);
        StartCoroutine(ProcessFullComboBox(isSpecialCombo, comboSnapshot));
    }

    private bool CheckForSpecialCombo(List<MiniGameUnloadBox> boxList)
    {
        // 리스트가 비어있거나, 3개가 아니거나, 중간에 null(실패)이 있으면 스페셜 콤보가 아님
        if (boxList == null || boxList.Count < 3 || boxList.Contains(null))
        {
            return false;
        }

        // 첫 번째 박스의 타입을 기준으로 삼음
        var firstBoxType = boxList[0].BoxType;

        // 나머지 박스들이 첫 번째 박스와 타입이 같은지 확인
        for (int i = 1; i < boxList.Count; i++)
        {
            if (boxList[i].BoxType != firstBoxType)
            {
                return false; // 다른 타입의 박스가 있으면 일반 콤보
            }
        }

        // 모든 박스가 같은 타입이면 스페셜 콤보
        return true;
    }

    private IEnumerator ProcessFullComboBox(bool isSpecialCombo, List<MiniGameUnloadBox> comboSnapshot)
    {
        if (comboSnapshot == null || comboSnapshot.Count == 0)
        {
            yield break;
        }

        // 1. 추가 점수 부여 (기본 300점)
        int bonusScore = 300;

        // 스페셜 콤보(같은 종류 3개)일 경우 점수 2배
        if (isSpecialCombo)
        {
            bonusScore *= 2;
            // 여기에 추가적인 시각/사운드 효과를 넣으면 더 좋습니다!
        }

        _score.AddScore(bonusScore);
        GenerateComboTextObj(bonusScore);

        // 2. 플레이어가 UI를 볼 수 있도록 1초 대기
        yield return new WaitForSeconds(1.0f);
    }
    public bool TryReserveHiddenBoxSpawn(MiniGameUnloadBoxSpawnPoint spawnPoint)
    {
        if (Managers.Stage.CurrentStageType != Define.ChapterType.End)
        {
            return false;
        }
        
        if (!IsEndingHiddenSpawnPoint(spawnPoint))
        {
            return false;
        }

        if (_hiddenBoxSpawnInterval <= 0 || _hiddenBoxReservedCount >= HiddenBoxTargetCount)
        {
            return false;
        }

        int nextHiddenSpawnCount = (_hiddenBoxReservedCount + 1) * _hiddenBoxSpawnInterval;
        if (_endingHiddenSpawnPointBoxCount + 1 < nextHiddenSpawnCount)
        {
            return false;
        }

        _hiddenBoxReservedCount++;
        return true;
    }

    public void NotifyBoxSpawned(MiniGameUnloadBoxSpawnPoint spawnPoint, MiniGameUnloadBox box)
    {
        _totalSpawnedBoxCount++;
        if (IsEndingHiddenSpawnPoint(spawnPoint))
        {
            _endingHiddenSpawnPointBoxCount++;
        }

        if (box != null && box.Info.IsHidden)
        {
            _hiddenBoxSpawnedCount++;
            Logger.Log($"Hidden Box Spawned ({_hiddenBoxSpawnedCount}/{HiddenBoxTargetCount})");
        }
    }

    public void CancelReservedHiddenBoxSpawn()
    {
        if (_hiddenBoxReservedCount > _hiddenBoxSpawnedCount)
        {
            _hiddenBoxReservedCount--;
        }
    }

    private bool IsEndingHiddenSpawnPoint(MiniGameUnloadBoxSpawnPoint spawnPoint)
    {
        if (Managers.Stage == null || Managers.Stage.CurrentStageType != Define.ChapterType.End)
        {
            return false;
        }

        if (spawnPoint == null)
        {
            return false;
        }

        if (_endingHiddenBoxSpawnPoints != null && _endingHiddenBoxSpawnPoints.Count > 0)
        {
            return _endingHiddenBoxSpawnPoints.Contains(spawnPoint);
        }

        return true;
    }

    private void OnDisposedBox(MiniGameUnloadBox box)
    {
        if (Managers.Stage.CurrentStageType != Define.ChapterType.End)
        {
            return;
        }

        if (box != null && box.Info.IsHidden)
        {
            _hiddenBoxDisposedCount++;
            Logger.Log($"Hidden Box Disposed ({_hiddenBoxDisposedCount}/{HiddenBoxTargetCount})");
        }
    }

    public bool PauseGame()
    {
        if (!IsActive)
        {
            Logger.LogWarning("Not Active MiniGame");
            return false;
        }
        IsPause = true;
        Logger.Log("UnloadGame Pausing game");
        return true;
    }

    public void ResumeGame()
    {
        if (!IsActive)
        {
            Logger.LogWarning("Not Active MiniGame");
            return;
        }
        IsPause = false;
        Logger.Log("UnloadGame Resuming game");
    }

    public void EndGame()
    {
        Managers.Sound.StopBGM();
        
        if (!IsActive)
        {
            Logger.LogWarning("Not Active MiniGame");
            return;
        }

        // 남은 폐기 박스 점수 감소
        if (_returnPoint != null)
        {
            _returnPoint.ReturnBoxScore();
        }

        IsActive = false;

        // 게임 종료 시 플레이어 캐릭터 애니메이션 설정
        if (PlayerCharacter != null)
        {
            PlayerCharacter.PlayWinPose();
        }

        int totalScore = _score.CurrentScore;
        var stageData = Managers.Stage.GetCurrentStageData();
        int preStarCount = Managers.Player.GetStageClearInfo(Managers.Stage.CurrentStageType);
        int starCount = Managers.Stage.CompleteStage(totalScore, preStarCount);

        float gameDuration = UnityEngine.Time.realtimeSinceStartup - _gameStartTime;
        string stageId = Managers.Stage.CurrentStageType.ToString();
        Managers.Analytics.TrackMinigameResult(stageId, "Unload", totalScore, gameDuration, starCount > 0, _comboSystem.MaxCombo);
        if (starCount > 0)
            Managers.Analytics.TrackStageClear(stageId, totalScore, starCount, gameDuration);
        else
            Managers.Analytics.TrackStageFail(stageId, totalScore, gameDuration);

        if (Managers.Stage.CurrentStageType == Define.ChapterType.End && starCount > 0)
        {
            Define.EventDataID endingEventID = stageData.GetEndingEventByHiddenBoxDisposedCount(_hiddenBoxDisposedCount);
            int endingThumbnailIndex = stageData.GetEndingThumbnailIndexByHiddenBoxDisposedCount(_hiddenBoxDisposedCount);
            Managers.Stage.SetEndingResult(endingEventID, endingThumbnailIndex);

            Logger.Log($"Ending Branch | hiddenDisposed: {_hiddenBoxDisposedCount}, event: {endingEventID}, thumbnailIndex: {endingThumbnailIndex}");
        }

        int totalGold = Managers.Stage.GetCompleteTotalGold(starCount);
        int additionalStars = Mathf.Max(0, starCount - preStarCount);
        if (additionalStars > 0)
        {
            Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, additionalStars);
        }
        int[] scoreList = stageData.ClearScoreList;
        int clearReward = stageData.ClearReward;
        int scoreBonus = totalGold;
        totalGold = totalGold + stageData.MinimumWage;
        
        int statsBonus = (int)(totalGold * Managers.Player.GetExperienceStatsBonusRate());
        totalGold = totalGold + statsBonus;

        Managers.Player.AddGold(totalGold, "stage_reward", Managers.Stage.CurrentStageType.ToString(), Managers.Stage.CurrentStageType.ToString());
        Managers.Analytics.Flush();

        Logger.Log($"Stage Result | starCount : {starCount}, totalGold : {totalGold}, totalScore : {totalScore}, statsBonus : {statsBonus}");

        var resultPopup = Managers.UI.ShowPopUI<UIGameUnloadResultPopup>();
        resultPopup.SetResultScore(_score.CurrentScore, totalGold, preStarCount, starCount, scoreList, clearReward, stageData.MinimumWage, scoreBonus, statsBonus);
        
        Logger.Log("UnloadGame Ending game");
    }
    public void AddScore(int score, MiniGameUnloadBox box)
    {
        if (score > 0)
        {
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString());
            
            // 1. 점수 반영
            GenerateScoreTextObj(score);
            _score.AddScore(score);

            // 2. 콤보 등록
            _comboSystem.RegisterSuccess(box);

        }
        else // 점수가 0 이하일 경우 (마이너스 점수)
        {
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.MinusScore.ToString());

            // 1. 감점은 배율 없이 그대로 적용
            GenerateScoreTextObj(score);
            _score.AddScore(score);

            // 2. 콤보 초기화   
            _comboSystem.BreakCombo();
        }
    }

    private void GenerateScoreTextObj(int amount)
    {
        InGameTextIndicator scoreTextObj = Managers.Resource.Instantiate("ScoreTextObj", transform).GetOrAddComponent<InGameTextIndicator>();
        string text = "";
        if (amount < 0)
        {
            text = $"<color=#BF0000>{amount}</color>";
        }
        else
        {
            text = $"<color=#006306>{amount}</color>";
        }
        
        scoreTextObj.Init(PlayerCharacter.transform.position, text);
    }

    private void GenerateComboTextObj(int amount)
    {
        InGameTextIndicator scoreTextObj = Managers.Resource.Instantiate("ScoreTextObj", transform).GetOrAddComponent<InGameTextIndicator>();
        string text = $"<color=#FF6F00>COMBO {amount}</color>";
        scoreTextObj.Init(PlayerCharacter.transform.position, text);
    }

}
