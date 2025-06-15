using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    private IMiniGame _currentGame;
    private GameObject _gameUI;
    
    public IMiniGame CurrentGame
    {
        get { return _currentGame; }
    }
    
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@MiniGameRoot");
            if (root == null)
            {
                root = new GameObject { name = "@MiniGameRoot" };
            }

            return root;
        }
    }
    
    public void Init()
    {
    }
    
    // 미니게임을 선택 및 생성하는 팩토리 메서드
    public void SelectMiniGame(Define.MiniGameType gameType)
    {
        switch (gameType)
        {
            case Define.MiniGameType.Unload:
                _currentGame = Root.GetOrAddComponent<MiniGameUnload>();
                break;
            case Define.MiniGameType.Delivery:
                _currentGame = Root.GetOrAddComponent<MiniGameDelivery>();
                break;
            // 새로운 미니게임을 추가하려면 여기에서 case 추가
            default:
                Logger.LogError("Unknown game type!");
                break;
        }
        
        InitializeUI();
        StartGame();
    }
    
    public void StartGame()
    {
        Logger.Log($"{_currentGame.GetType().Name} | Game Start");
        
        // 설정된 카메라 값으로 변경한다.
        Managers.Camera.Init(CurrentGame.CameraType, CurrentGame.CameraSettings);

        UIGameStartPopup uIGameStart = Managers.UI.ShowPopUI<UIGameStartPopup>();
        uIGameStart.PlayGameStartEffect(_currentGame.StartGame);
    }

    public bool PauseGame()
    {
        Logger.Log("Pause Game");
        return _currentGame.PauseGame();
    }

    public void ResumeGame()
    {
        Logger.Log("Resume Game");
        _currentGame?.ResumeGame();
    }

    public void EndGame()
    {
        Logger.Log("End Game");
        Managers.Player.AddStat(Define.PlayerStatType.Fatigue, -30);
        _currentGame?.EndGame();
    }
    
    private void InitializeUI()
    {
        _currentGame?.InitializeUI();
    }

}
