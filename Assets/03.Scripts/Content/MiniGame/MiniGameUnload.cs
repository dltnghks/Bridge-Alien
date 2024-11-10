using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnload : IMiniGame
{
    private int _score = 0;
    private int _gameTime = 5;
    public UIScene GameUI { get; set; }
    private UIGameUnloadScene _uiGameUnloadScene;
        
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

        _uiGameUnloadScene.SetGame(_gameTime, EndGame);
    }
}
