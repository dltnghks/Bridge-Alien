using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MiniGameUnload : MonoBehaviour, IMiniGame
{
    [Header("Game Setting")]
    [SerializeField] private MiniGameUnloadSetting _gameSetting;
    [SerializeField] private SkillBase[] _skillList; // 스킬 리스트

    [Header("Delivery Point")]
    [SerializeField] private List<MiniGameUnloadDeliveryPoint> _deliveryPointList = new List<MiniGameUnloadDeliveryPoint>();
    [SerializeField] private MiniGameUnloadCoolingPoint _coolingPoint;

    [Header("Box Spawn Point")]
    [SerializeField] private MiniGameUnloadBoxSpawnPoint _boxSpawnPoint;                   // 박스 생성 주기
    [SerializeField] private GameObject[] _boxPrefabList;

    [Header("Return Point")]
    [SerializeField] private MiniGameUnloadReturnPoint _returnPoint;

    [Header("Disposal Point")]
    [SerializeField] private MiniGameUnloadDisposalPoint _disposePoint;

    private ComboSystem _comboSystem;

    public bool IsActive { get; set; }
    public bool IsPause { get; set; }

    public Player PlayerCharacter { get; set; }
    public IPlayerController PlayerController { get; set; }

    public UIScene GameUI { get; set; }
    private UIGameUnloadScene _uiGameUnloadScene;

    private TimerBase _timer;
    private ScoreBase _score;
    private MiniGameUnloadBoxPreview _boxPreview;
    private readonly int _minimumWage = 10000;

    public void InitializeUI()
    {
        Logger.Log("InitializeUI Starting game");

        _uiGameUnloadScene = Managers.UI.ShowSceneUI<UIGameUnloadScene>();
        GameUI = _uiGameUnloadScene;
        GameUI.Init();
    }

    private void Update()
    {
        if (!IsActive || IsPause)
        {
            return;
        }

        _timer.TimerUpdate();
        _boxPreview.TimerUpdate();
    }

    public void StartGame()
    {
        Logger.Log("UnloadGame Starting game");

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
        Managers.Camera.Initialize(PlayerCharacter.transform);

        // 게임 활성화
        IsActive = true;

        Logger.Log("Game successfully started.");

        StartTutorial();
    }

    public void StartTutorial()
    {
        // 도움말을 처음보는 경우 띄워주기
        if (Managers.MiniGame.MiniGameTutorial[(int)Define.MiniGameType.Unload] == false)
        {
            Managers.UI.ClosePopupUI();
            Managers.MiniGame.MiniGameTutorial[(int)Define.MiniGameType.Unload] = true;
            Managers.UI.ShowPopUI<UITutorialPopup>("UIMGUTutorialPopup");
        }
    }

    private void SetGameInfo()
    {
        _gameSetting = Managers.Data.MiniGameData.GetMiniGameSettings<MiniGameUnloadSetting>(Define.MiniGameType.Unload);

        if (_gameSetting == null)
        {
            Logger.LogError("Not Found Game Information");
        }
    }

    private void SetColdArea()
    {
        if (_coolingPoint == null)
        {
            Logger.LogError("Failed to get or add MiniGameUnloadBoxSpawnPoint component!");
            return;
        }

        _coolingPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
        _coolingPoint.SetColdArea(_gameSetting.MaxColdAreaBoxIndex);
    }

    private void SetDeliveryPointList()
    {
        // DeliveryPoint 초기화
        foreach (var deliveryPoint in _deliveryPointList)
        {
            if (deliveryPoint == null)
            {
                Logger.LogError("Null deliveryPoint encountered!");
                continue;
            }

            deliveryPoint.OnScoreAction += AddScore;
            deliveryPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
            deliveryPoint.OnReturnAction += _returnPoint.PlaceBox;
            deliveryPoint.SetAction();
        }
    }

    private void SetDisposalPoint()
    {
        _disposePoint.OnScoreAction += AddScore;
        _disposePoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
    }

    private void SetSpawnBoxList()
    {
        if (_boxSpawnPoint == null)
        {
            Logger.LogError("Failed to get or add MiniGameUnloadBoxSpawnPoint component!");
            return;
        }

        _boxSpawnPoint.OnTriggerAction += _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite;
        _boxSpawnPoint.SetBoxSpawnPoint(_gameSetting.MaxSpawnBoxIndex);
    }

    private void SetReturnPoint()
    {
        if (_returnPoint == null)
        {
            Logger.LogError("Failed to get or add MiniGameUnloadBoxSpawnPoint component!");
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

        PlayerController = new MiniGameUnloadPlayerController(PlayerCharacter, _gameSetting.DetectionBoxRadius, _gameSetting.MoveSpeedReductionRatio, _boxSpawnPoint, _coolingPoint, _uiGameUnloadScene.UIBoxPreview.UpdateUI);
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
        _boxPreview = Utils.GetOrAddComponent<MiniGameUnloadBoxPreview>(gameObject);

        _score.OnChangedScore += _uiGameUnloadScene.UIScoreBoard.SetScore;
        _timer.OnChangedTime += _uiGameUnloadScene.UITimer.SetTimerText;
        _timer.OnEndTime += EndGame;

        _timer.SetTimer(_gameSetting.GamePlayTime);

        _boxPreview.SetBoxPreview(_gameSetting.BoxSpawnInterval, _boxSpawnPoint, _boxPrefabList);

        _comboSystem = new ComboSystem();
        if (_comboSystem != null && _uiGameUnloadScene.UIComboDisplay != null)
        {
            _comboSystem.OnComboChanged += _uiGameUnloadScene.UIComboDisplay.UpdateCombo;
            _comboSystem.OnComboBroken += _uiGameUnloadScene.UIComboDisplay.BreakCombo;
            _comboSystem.OnChangedComboBox += _uiGameUnloadScene.UIComboBoxView.UpdateUI;
        }
    }

    private void SetComboSystem()
    {
        _comboSystem = new ComboSystem();
    }

    public bool PauseGame()
    {
        if (!IsActive || IsPause)
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
        if (!IsActive || !IsPause)
        {
            Logger.LogWarning("Not Active MiniGame");
            return;
        }
        IsPause = false;
        Logger.Log("UnloadGame Resuming game");
    }

    public void EndGame()
    {
        if (!IsActive)
        {
            Logger.LogWarning("Not Active MiniGame");
            return;
        }

        // 남은 폐기 박스 점수 감소
        _returnPoint.ReturnBoxScore();

        IsActive = false;

        // 게임 종료 시 플레이어 캐릭터 애니메이션 설정
        PlayerCharacter.PlayWinPose();

        float experienceBonus = Managers.Player.GetExperienceStatsBonus() * 100f;
        float fatiguePenalty = Managers.Player.GetFatigueStatsPenalty() * 100f;
        float scoreBonus = _score.CurrentScore * 0.1f;
        float totalScore = _minimumWage * (scoreBonus + experienceBonus - fatiguePenalty) / 100f;

        Managers.Player.AddGold((int)totalScore);

        Managers.UI.ShowPopUI<UIGameUnloadResultPopup>().SetResultScore(_score.CurrentScore, _minimumWage, experienceBonus, fatiguePenalty, scoreBonus, totalScore);
        Logger.Log("UnloadGame Ending game");
    }

    public void AddScore(int score, MiniGameUnloadBox box)
    {
        if (score > 0)
        {
            // 1. 콤보 등록
            _comboSystem.RegisterSuccess(box);

            // 2. 콤보 배율 적용
            float multiplier = _comboSystem.GetScoreMultiplier();
            int finalScore = Mathf.RoundToInt(score * multiplier);

            // 3. 최종 점수 반영
            GenerateScoreTextObj(finalScore);
            _score.AddScore(finalScore);
        }
        else // 점수가 0 이하일 경우 (마이너스 점수)
        {
            // 1. 콤보 초기화
            _comboSystem.BreakCombo();

            // 2. 감점은 배율 없이 그대로 적용
            GenerateScoreTextObj(score);
            _score.AddScore(score);
        }
    }
    
    private void GenerateScoreTextObj(int amount)
    {
        InGameTextIndicator scoreTextObj = Managers.Resource.Instantiate("ScoreTextObj", transform).GetOrAddComponent<InGameTextIndicator>();
        scoreTextObj.Init(PlayerCharacter.transform.position, amount);
    }

}
