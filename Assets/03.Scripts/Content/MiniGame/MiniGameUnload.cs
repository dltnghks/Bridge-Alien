using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameUnload : MonoBehaviour, IMiniGame, IScorable, ITimed
{
    // 게임을 플레이할 수 있는 시간
    private int _gameTime = 5;
    
    public int CurrentScore { get; set; }
    
    public bool IsActive { get; set; }
    public float CurTime { get; set; }
    public UnityAction EndTimerAction { get; set; }
    
    public UIScene GameUI { get; set; }
    private UIGameUnloadScene _uiGameUnloadScene;

    private void Update()
    {
        TimerUpdate();
    }
    
    public void StartGame()
    {
        Debug.Log("UnloadGame Starting game");
    }

    public void PauseGame()
    {
        Debug.Log("UnloadGame Pausing game");
    }

    public void ResumeGame()
    {
        Debug.Log("UnloadGame Resuming game");
    }

    public void EndGame()
    {
        Managers.Scene.ChangeScene(Define.Scene.GameMap);
        Debug.Log("UnloadGame Ending game");
    }

    public void InitializeUI()
    {
        Debug.Log("InitializeUI Starting game");
        _uiGameUnloadScene = Managers.UI.ShowSceneUI<UIGameUnloadScene>();
        GameUI = _uiGameUnloadScene;
        
        SetTimer(_gameTime, EndGame);
        SetScore(0);
    }

    public void SetTimer(float time, UnityAction endTimerAction = null)
    {
        IsActive = true;
        CurTime = time;
        EndTimerAction = endTimerAction;
        _uiGameUnloadScene.UITimer.SetTimer(_gameTime);
    }
    
    public void TimerUpdate()
    {
        float deltaTime = -Time.deltaTime;
        CurTime += deltaTime;
        AddTime(deltaTime);
            
        if (CurTime <= 0 && IsActive)
        {
            EndTimer();
        }
    }

    public void AddTime(float time)
    {
        _uiGameUnloadScene.UITimer.AddTime(time);
    }

    public void EndTimer()
    {
        if (IsActive)
        {
            EndTimerAction?.Invoke();
            IsActive = false;
        }
    }

    public void SetScore(int score)
    {
        CurrentScore = score;
        _uiGameUnloadScene.UIScoreBoard.SetScore(CurrentScore);
    }

    public void AddScore(int score)
    {
        CurrentScore += score;
        _uiGameUnloadScene.UIScoreBoard.SetScore(CurrentScore);
    }
}
