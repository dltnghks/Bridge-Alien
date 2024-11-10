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
    
    public void Init()
    {
    }
    
    // 미니게임을 선택 및 생성하는 팩토리 메서드
    public void SelectMiniGame(Define.MiniGameType gameType)
    {
        switch (gameType)
        {
            case Define.MiniGameType.Unload:
                _currentGame = Utils.GetOrAddComponent<MiniGameUnload>(gameObject);
                break;
            // 새로운 미니게임을 추가하려면 여기에서 case 추가
            default:
                Debug.LogError("Unknown game type!");
                break;
        }
        
        InitializeUI();
        StartGame();
    }
    
    public void StartGame()
    {
        _currentGame?.StartGame();
    }

    public void PauseGame()
    {
        _currentGame?.PauseGame();
    }

    public void ResumeGame()
    {
        _currentGame?.ResumeGame();
    }

    public void EndGame()
    {
        _currentGame?.EndGame();
    }
    
    private void InitializeUI()
    {
        _currentGame?.InitializeUI();
    }

}
