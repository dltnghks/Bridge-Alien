using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public void StartGame()
    {
        // PlayerCharacter 초기화
        PlayerCharacter = GameObject.Find("Player")?.GetComponent<Player>();
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

    public void PauseGame()
    {
        throw new System.NotImplementedException();
    }

    public void ResumeGame()
    {
        throw new System.NotImplementedException();
    }

    public void EndGame()
    {
        throw new System.NotImplementedException();
    }
}
