using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDelivery : MonoBehaviour, IMiniGame
{
	// 데이터 매니저에서 가져올 게임 데이터
	[Header("Game Setting")]
    [SerializeField] private MiniGameDeliverySetting _gameSetting;
    
    [Header("게임 종료 시간")]
    [SerializeField] private float gameTime = 1500f;

    [Header("카메라 값 세팅")]
    [SerializeField] private CameraManager.CameraType cameraType;
    [SerializeField] private CameraSettings cameraSettings;

    [Header("플레이어 최대 이동 거리")]
    [SerializeField] private float maxDistance = .0f;
    [SerializeField] private float totalDistance = .0f;
    
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
    
    private InfiniteMap _infiniteMap;
    
    private void Update()
    {
        if (!IsActive || IsPause)
        {
            return;
        }

        _pathProgressBar?.ProgressUpdate(totalDistance);
    }
    
    public void StartGame()
    {
        // ProgressBar 값 초기화
        // MiniGameDeliveryPathProgress : 실질적인 Progress의 상태를 관리하고 있음
        _pathProgressBar = new MiniGameDeliveryPathProgress();
        _pathProgressBar.SetProgressBar(_uiGameDeliveryScene.UIPathProgressBar, maxDistance, EndGame);
        
        PlayerCharacter = Utils.FindChild<MiniGameDeliveryPlayer>(gameObject, "Player", true);
        if (PlayerCharacter == null)
        {
            Debug.Log("Player 캐릭터가 존재하지 않아요.");
            return;
        }

        PlayerController = new MiniGameDeliveryPlayerController(PlayerCharacter);
        if (PlayerController == null)
        {
            Debug.Log("플레이어 컨트롤러가 존재하지 않아요.");
            return;
        }

        _infiniteMap = Utils.FindChild<InfiniteMap>(gameObject, "Map", true);
        if (_infiniteMap == null)
        {
            Debug.Log("맵 생성 스크립트가 존재하지 않아요.");
            return;
        }

        // UpdateDistanceFromMap은 Delivery에서 값을 가지고 있기 위해서
        // 증가 값을 넘겨준다.
        _infiniteMap.InitializeMap(maxDistance, UpdateDistanceFromMap);
        IsActive = true;
    }
    
    private void SetGameInfo()
    {
        // 카메라 세팅 값을 가져와서 게임 세팅하는 듯.
        _gameSetting = Managers.Data.MiniGameData.GetMiniGameSettings<MiniGameDeliverySetting>(Define.MiniGameType.Delivery);
        
        if (_gameSetting == null)
        {
            Logger.LogError("Not Found Game Information");
        }
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

    public void UpdateTotalDistance(float distance)
    {
        totalDistance = distance;
    }

    public float GetDistance()
    {
        return totalDistance;
    }

    private void UpdateDistanceFromMap(float distance)
    {
        UpdateTotalDistance(distance);
    }
}
