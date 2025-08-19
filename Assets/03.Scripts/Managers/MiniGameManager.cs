using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour, ISaveable
{
    private IMiniGame _currentGame;
    private GameObject _gameUI;
    private Define.MiniGameType _currnetMiniGameType = Define.MiniGameType.Unknown;
    public bool[] MiniGameTutorial = new bool[(int)Define.MiniGameType.Delivery + 1];

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

    public void Init(bool[] miniGameTutorial = null)
    {
        if (miniGameTutorial is null || miniGameTutorial.Length == 0)
        {
            MiniGameTutorial = new bool[(int)Define.MiniGameType.Delivery + 1];

            Logger.Log($"1. Init MiniGame Tutorial : {Managers.MiniGame.MiniGameTutorial.Length}");

        }
        else
        {
            MiniGameTutorial = miniGameTutorial;
            Logger.Log($"2. Init MiniGame Tutorial : {Managers.MiniGame.MiniGameTutorial.Length}");
        }
    }

    
    // 미니게임을 선택 및 생성하는 팩토리 메서드
    public void SelectMiniGame(Define.MiniGameType gameType)
    {
        switch (gameType)
        {
            case Define.MiniGameType.Unload:
                _currentGame = Root.GetComponentInChildren<MiniGameUnload>();
                break;
            // 새로운 미니게임을 추가하려면 여기에서 case 추가
            default:
                Logger.LogError("Unknown game type!");
                break;
        }

        _currnetMiniGameType = gameType;

        InitializeUI();
        Logger.Log($"{_currentGame.GetType().Name} | Game Start");

        UIGameStartPopup uIGameStart = Managers.UI.ShowPopUI<UIGameStartPopup>();
        uIGameStart.PlayGameStartEffect(StartGame);
    }

    public void StartGame()
    {
        _currentGame.StartGame();
        
        // 배경음 재생
        string sceneTypeStr =  System.Enum.GetName(typeof(Define.Scene), Managers.Scene.CurrentSceneType);
        Managers.Sound.PlayBGM(sceneTypeStr);
    }

    public bool PauseGame()
    {
        Logger.Log("Pause Game");
        Managers.Sound.PauseBGM();

        if (_currentGame is null)
        {
            return false;
        }

        return _currentGame.PauseGame();
    }

    public void ResumeGame()
    {
        Logger.Log("Resume Game");
        Managers.Sound.ResumeBGM();

        _currentGame?.ResumeGame();
    }

    public void EndGame()
    {
        Logger.Log("End Game");
        _currentGame?.EndGame();
        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, -30);
    }
    
    private void InitializeUI()
    {
        _currentGame?.InitializeUI();
    }

    public void Add(ISaveable saveable)
    {
        throw new System.NotImplementedException();
    }

    public object CaptureState()
    {
        var data = new MiniGameSaveData();
        data.MiniGameTutorial = MiniGameTutorial;
        return data;
    }

    public void RestoreState(object state)
    {
        var data = state as MiniGameSaveData;
        if (data == null)
        {
            data = new MiniGameSaveData();
            data.MiniGameTutorial = null;
        }
        Init(data.MiniGameTutorial);
    }
}
