using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameDeliveryScene : UIScene
{
    enum Objects
    {
        UITimer,
        UIScoreBoard,
        UIGameMenu,
        UIMiniGameDeliveryPlayerInput,
    }
    
    private UITimer _uiTimer;
    private UIScoreBoard _uiScoreBoard;
    private UIGameMenu _uiGameMenu;
    private UIMiniGameDeliveryPlayerInput _uiPlayerInput;
    
    public UITimer UITimer { get { return _uiTimer; } }
    public UIScoreBoard UIScoreBoard { get { return _uiScoreBoard; } }
    public UIMiniGameDeliveryPlayerInput UIPlayerInput { get { return _uiPlayerInput; } }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));
        
        _uiTimer = GetObject((int)Objects.UITimer).GetOrAddComponent<UITimer>();
        _uiScoreBoard = GetObject((int)Objects.UIScoreBoard).GetOrAddComponent<UIScoreBoard>();
        _uiGameMenu = GetObject((int)Objects.UIGameMenu).GetOrAddComponent<UIGameMenu>();
        _uiPlayerInput = GetObject((int)Objects.UIMiniGameDeliveryPlayerInput).GetOrAddComponent<UIMiniGameDeliveryPlayerInput>();
        
        return true;
    }
}
