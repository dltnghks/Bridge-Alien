using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniGameDelivery : MonoBehaviour, IMiniGame
{
    [Header("Game Information")]
    // 게임을 플레이할 수 있는 시간
    [SerializeField] private float _gameTime = 60.0f;

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
    private UIGameDeliveryScene _uiGameDeliveryScene;
 
    private MiniGameDeliveryPathProgress _pathPrgressBar;
    
    private void Update()
    {
        if (!IsActive || IsPause)
        {
            return;
        }

        _pathPrgressBar?.ProgressUpdate();
    }
    
    
    public void StartGame()
    {
        // 게임에 사용되는 요소들 초기화
        _pathPrgressBar = new MiniGameDeliveryPathProgress();
        _pathPrgressBar.SetProgressBar(_uiGameDeliveryScene.UIPathProgressBar, _gameTime, EndGame);
    

        
        
        // PlayerCharacter 초기화
        PlayerCharacter = Utils.FindChild<MiniGameDeliveryPlayer>(gameObject, "MiniGameDeliveryPlayer", true);
        if (PlayerCharacter == null)
        {
            Logger.LogError("PlayerCharacter not found or does not have a Player component!");
            return;
        }
        
        PlayerController = new MiniGameDeliveryPlayerController(PlayerCharacter);
        if (PlayerController == null)
        {
            Logger.LogError("Failed to initialize PlayerController!");
            return;
        }
        PlayerController.Init(PlayerCharacter);
        
        // 게임 활성화
        IsActive = true;
    }

    public bool PauseGame()
    {
        if (!IsActive || IsPause)
        {
            Logger.LogError("Not Active MiniGame");
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
        Managers.UI.ShowPopUI<UIGameUnloadResultPopup>().SetResultScore((int)_pathPrgressBar.CurValue);
        Logger.Log("UnloadGame Ending game");
    }
    
    public void InitializeUI()
    {
        Logger.Log("InitializeUI Starting game");
        _uiGameDeliveryScene = Managers.UI.ShowSceneUI<UIGameDeliveryScene>();
        GameUI = _uiGameDeliveryScene;
        GameUI.Init();
    }
}
