using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIGameUnloadScene : UIScene
{
    enum Objects
    {
        UITimer,
        UIScoreBoard,
        UIBoxPreview,
        UIGameMenu,
        UIMiniGameUnloadPlayerInput,
    }
    
    private UITimer _uiTimer;
    private UIScoreBoard _uiScoreBoard;
    private UIBoxPreview _uiBoxPreview;
    private UIGameMenu _uiGameMenu;
    private UIMiniGameUnloadPlayerInput _uiPlayerInput;
    public UITimer UITimer { get { return _uiTimer; } }
    public UIScoreBoard UIScoreBoard { get { return _uiScoreBoard; } }
    public UIBoxPreview UIBoxPreview { get { return _uiBoxPreview; } }
    public UIMiniGameUnloadPlayerInput UIPlayerInput { get { return _uiPlayerInput; } }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));
        
        _uiTimer = GetObject((int)Objects.UITimer).GetOrAddComponent<UITimer>();
        _uiScoreBoard = GetObject((int)Objects.UIScoreBoard).GetOrAddComponent<UIScoreBoard>();
        _uiBoxPreview = GetObject((int)Objects.UIBoxPreview).GetOrAddComponent<UIBoxPreview>();
        _uiGameMenu = GetObject((int)Objects.UIGameMenu).GetOrAddComponent<UIGameMenu>();
        _uiPlayerInput = GetObject((int)Objects.UIMiniGameUnloadPlayerInput).GetOrAddComponent<UIMiniGameUnloadPlayerInput>();
        
        return true;
    }
    
}
