using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MiniGameUnload : MonoBehaviour, IMiniGame
{
    [Header("Game Information")]
    // 게임을 플레이할 수 있는 시간
    [SerializeField] private float _gameTime = 6.0f;
    // 박스 생성 주기
    [SerializeField] private float _boxSpawnInterval = 3.0f;

    [Header("Delivery Point")]
    [SerializeField] private List<MiniGameUnloadDeliveryPoint> _deliveryPointList = new List<MiniGameUnloadDeliveryPoint>();

    [Header("Box Spawn Point")]
    [SerializeField] private MiniGameUnloadBoxSpawnPoint _boxSpawnPoint;    
    [SerializeField] private int _maxSpawnBoxIndex = 3;

    [Header("Player Information")]
    [SerializeField] private float _detectionBoxRadius = 2f;
    [SerializeField] private float _moveSpeedReductionRatio = 2.0f;


    public bool IsActive { get; set; }
    public bool IsPause { get; set; }

    public Player PlayerCharacter { get; set; }
    public IPlayerController PlayerController { get; set; }
    
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

        GameObject deliveryPoinListObj = Utils.FindChild(gameObject, "DeliveryPointList", true);
        Logger.Log(deliveryPoinListObj.name);
        foreach(var deliveryPoint in deliveryPoinListObj.GetComponentsInChildren<MiniGameUnloadDeliveryPoint>()){
            deliveryPoint.SetAction(AddScore, _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite);
            _deliveryPointList.Add(deliveryPoint);
        }

        GameObject boxSpawnPointObj = Utils.FindChild(gameObject, "BoxSpawnPoint", true);
        _boxSpawnPoint = boxSpawnPointObj.GetOrAddComponent<MiniGameUnloadBoxSpawnPoint>();
        _boxSpawnPoint.SetBoxSpawnPoint(_maxSpawnBoxIndex, _uiGameUnloadScene.UIPlayerInput.SetInteractionButtonSprite);

        _timer = new TimerBase();
        _score = new ScoreBase();
        _boxPreview =  Utils.GetOrAddComponent<MiniGameUnloadBoxPreview>(gameObject);
        
        _timer.SetTimer(_uiGameUnloadScene.UITimer, _gameTime, EndGame);
        _score.SetScore(_uiGameUnloadScene.UIScoreBoard, 0);
        _boxPreview.SetBoxPreview(_uiGameUnloadScene.UIBoxPreview, _boxSpawnInterval, _boxSpawnPoint);
        
        PlayerCharacter = GameObject.Find("Player").GetComponent<Player>();
        PlayerController = new MiniGameUnloadPlayerController(PlayerCharacter, _detectionBoxRadius, _moveSpeedReductionRatio, _boxSpawnPoint);
        PlayerController.Init(PlayerCharacter);

        IsActive = true;
    }

    public void PauseGame()
    {
        if (!IsActive || IsPause)
        {
            Logger.LogError("Not Active MiniGame");
            return;
        }
        IsPause = true;
        Logger.Log("UnloadGame Pausing game");
    }

    public void ResumeGame()
    {
        if (!IsActive || !IsPause)
        {
            Logger.LogError("Not Active MiniGame");
            return;
        }
        IsPause = false;
        Logger.Log("UnloadGame Resuming game");
    }

    public void EndGame()
    {
        if (!IsActive)
        {
            Logger.LogError("Not Active MiniGame");
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
    }

    public void AddScore(int weight)
    {
        int score = 0;
        int bonus = 0;
        if(weight > 0) bonus = 10;
        else bonus = 5;
             
        score = weight * bonus;
        _score.AddScore(score);
    }

}
