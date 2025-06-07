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

    [Header("Delivery Point")]
    [SerializeField] private List<MiniGameUnloadDeliveryPoint> _deliveryPointList = new List<MiniGameUnloadDeliveryPoint>();

    [Header("Box Spawn Point")]
    [SerializeField] private MiniGameUnloadBoxSpawnPoint _boxSpawnPoint;                   // 박스 생성 주기
    [SerializeField] private GameObject[] _boxPrefabList;

    [Header("Game Camera Settings")]
    [SerializeField] private CameraManager.CameraType _cameraType;
    [SerializeField] private CameraSettings _cameraSettings;
    
    public bool IsActive { get; set; }
    public bool IsPause { get; set; }

    public Player PlayerCharacter { get; set; }
    public IPlayerController PlayerController { get; set; }
    public CameraManager.CameraType CameraType
    {
        get { return _cameraType;}
        set { _cameraType = value; }
    }

    public CameraSettings CameraSettings
    {
        get { return _cameraSettings; }
        set { _cameraSettings = value; }
    }

    public UIScene GameUI { get; set; }
    private UIGameUnloadScene _uiGameUnloadScene;
 
    private TimerBase _timer;
    private ScoreBase _score;
    private MiniGameUnloadBoxPreview _boxPreview;
    
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

        SetDeliveryPointList();

        SetSpawnBoxList();
        
        SetPlayerCharacter();

        // 다른 설정이 끝난 후 UI 설정
        SetGameUI();
        
        // 게임 활성화
        IsActive = true;

        
        Logger.Log("Game successfully started.");
    }

    private void SetGameInfo()
    {
        _gameSetting = Managers.Data.MiniGameData.GetMiniGameSettings<MiniGameUnloadSetting>(Define.MiniGameType.Unload);
        
        if (_gameSetting == null)
        {
            Logger.LogError("Not Found Game Information");
        }
    }

    private void SetDeliveryPointList()
    {
        // DeliveryPointList 확인
        GameObject deliveryPoinListObj = Utils.FindChild(gameObject, "DeliveryPointList", true);
        if (deliveryPoinListObj == null)
        {
            Logger.LogError("DeliveryPointList object not found!");
            return;
        }
        Logger.Log($"Found DeliveryPointList: {deliveryPoinListObj.name}");

        // DeliveryPoint 초기화
        foreach (var deliveryPoint in deliveryPoinListObj.GetComponentsInChildren<MiniGameUnloadDeliveryPoint>())
        {
            if (deliveryPoint == null)
            {
                Logger.LogError("Null deliveryPoint encountered!");
                continue;
            }
            deliveryPoint.SetAction(AddScore, _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite);
            _deliveryPointList.Add(deliveryPoint);
        }
    }
    
    private void SetSpawnBoxList()
    {
        GameObject boxSpawnPointObj = Utils.FindChild(gameObject, "BoxSpawnPoint", true);
        if (boxSpawnPointObj == null)
        {
            Logger.LogError("BoxSpawnPoint object not found!");
            return;
        }
        _boxSpawnPoint = boxSpawnPointObj.GetOrAddComponent<MiniGameUnloadBoxSpawnPoint>();
        if (_boxSpawnPoint == null)
        {
            Logger.LogError("Failed to get or add MiniGameUnloadBoxSpawnPoint component!");
            return;
        }
        _boxSpawnPoint.SetBoxSpawnPoint(_gameSetting.MaxSpawnBoxIndex, _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite);

    }

    private void SetPlayerCharacter()
    {
        PlayerCharacter = GameObject.Find("Player")?.GetComponent<Player>();
        if (PlayerCharacter == null)
        {
            Logger.LogError("PlayerCharacter not found or does not have a Player component!");
            return;
        }

        PlayerController = new MiniGameUnloadPlayerController(PlayerCharacter, _gameSetting.DetectionBoxRadius, _gameSetting.MoveSpeedReductionRatio, _boxSpawnPoint);
        if (PlayerController == null)
        {
            Logger.LogError("Failed to initialize PlayerController!");
            return;
        }
        PlayerController.Init(PlayerCharacter);
    }

    // 게임 설정과 박스 스폰 리스트가 설정이 되어있을 때, UI 세팅
    private void SetGameUI()
    {
        // UI 초기화
        _timer = new TimerBase();
        _score = new ScoreBase();
        _boxPreview = Utils.GetOrAddComponent<MiniGameUnloadBoxPreview>(gameObject);

        _timer.SetTimer(_uiGameUnloadScene.UITimer, _gameSetting.GamePlayTime, EndGame);
        _score.SetScore(_uiGameUnloadScene.UIScoreBoard, 0);
        _boxPreview.SetBoxPreview(_uiGameUnloadScene.UIBoxPreview, _gameSetting.BoxSpawnInterval, _boxSpawnPoint, _boxPrefabList);
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
        
        IsActive = false;
        Managers.UI.ShowPopUI<UIGameUnloadResultPopup>().SetResultScore(_score.CurrentScore);
        Logger.Log("UnloadGame Ending game");
    }

    public void InitializeUI()
    {
        Logger.Log("InitializeUI Starting game");
        
        _uiGameUnloadScene = Managers.UI.ShowSceneUI<UIGameUnloadScene>();
        GameUI = _uiGameUnloadScene;
        GameUI.Init();
    }

    public void AddScore(int score)
    {
        if (score > 0)
        {
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString());
        }
        else if (score < 0)
        {
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.MinusScore.ToString());
        }

        _score.AddScore(score);
    }

}
