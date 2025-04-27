using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDelivery : MonoBehaviour, IMiniGame
{
    [Header("게임 종료 시간")]
    [SerializeField] private float gameTime = 60.0f;

    [Header("카메라 값 세팅")]
    [SerializeField] private CameraManager.CameraType cameraType;
    [SerializeField] private CameraSettings cameraSettings;
    
    public bool IsActive { get; set; }
    public bool IsPause { get; set; }

    public Player PlayerCharacter { get; set; }
    public IPlayerController PlayerController { get; set; }
    public CameraManager.CameraType CameraType
    {
        get { return cameraType;}
        set { cameraType = value; }
    }
    public CameraSettings CameraSettings
    {
        get { return cameraSettings; }
        set { cameraSettings = value; }
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
        // ProgressBar 값 초기화
        // MiniGameDeliveryPathProgress : 실질적인 Progress의 상태를 관리하고 있음
        _pathProgressBar = new MiniGameDeliveryPathProgress();
        _pathProgressBar.SetProgressBar(_uiGameDeliveryScene.UIPathProgressBar, gameTime, EndGame);
    
        PlayerCharacter = Utils.FindChild<MiniGameDeliveryPlayer>(gameObject, "Player", true);
        if (PlayerCharacter == null)
            return;
        
        PlayerController = new MiniGameDeliveryPlayerController(PlayerCharacter);
        if (PlayerController == null)
            return;
        
        IsActive = true;
    }

    public bool PauseGame()
    {
        if (!IsActive || IsPause)
        {
            return false;
        }
        IsPause = true;
        return true;
    }

    public void ResumeGame()
    {
        if (!IsActive || !IsPause)
        {
            return;
        }
        IsPause = false;
    }

    public void EndGame()
    {
        if (!IsActive)
        {
            return;
        }
        
        IsActive = false;
        Managers.UI.ShowPopUI<UIGameUnloadResultPopup>().SetResultScore((int)_pathProgressBar.CurValue * 100);
    }
    
    public void InitializeUI()
    {
        _uiGameDeliveryScene = Managers.UI.ShowSceneUI<UIGameDeliveryScene>();
        GameUI = _uiGameDeliveryScene;
        GameUI.Init();
    }
}
