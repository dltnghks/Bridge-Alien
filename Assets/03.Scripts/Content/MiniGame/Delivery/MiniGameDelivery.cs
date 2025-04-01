using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 미니 게임에서 중앙이 되는 스크립트
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
        get { return _cameraType; }
        set { _cameraType = value; }
    }

    public CameraSettings CameraSettings
    {
        get { return _cameraSettings; }
        set { _cameraSettings = value; }
    }

    public UIScene GameUI { get; set; }
    private UIGameDeliveryScene _uiGameDeliveryScene;
    private MiniGameDeliveryPathProgress _pathProgressBar;

    private void Update()
    {
        if (!IsActive || IsPause)
        {
            return;
        }

        _pathProgressBar?.ProgressUpdate();
    }


    public void StartGame()
    {

        // PlayerCharacter 초기화
        PlayerCharacter = Utils.FindChild<MiniGameDeliveryPlayer>(gameObject, "Player", true);
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

        // 게임에 사용되는 요소들 초기화
        _pathProgressBar = new MiniGameDeliveryPathProgress();
        _pathProgressBar.Initialize(_uiGameDeliveryScene.UIPathProgressBar, PlayerCharacter.transform, new Vector3(300f, 0f, 0f), EndGame);

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
        // Managers.UI.ShowPopUI<UIGameUnloadResultPopup>().SetResultScore((int)_pathProgressBar.CurValue * 100);
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
