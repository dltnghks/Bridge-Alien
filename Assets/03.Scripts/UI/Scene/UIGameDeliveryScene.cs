using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameDeliveryScene : UIScene
{
    enum Objects
    {
        UIPathProgressBar,
        UIGameMenu,
        UIMiniGameDeliveryPlayerInput,
        UIDamageView,
        UIMiniMiniGame
    }
    
    private UIGameMenu _uiGameMenu;
    private UIMiniGameDeliveryPlayerInput _uiPlayerInput;
    private UIPathProgressBar _uiPathProgressBar;
    private UIDamageView _uiDamageView;
    private UIMiniMiniGame _uiMiniMiniGame;
    
    public UIMiniGameDeliveryPlayerInput UIPlayerInput { get { return _uiPlayerInput; } }
    public UIPathProgressBar UIPathProgressBar { get { return _uiPathProgressBar;}}
    public UIDamageView UIDamageView { get { return _uiDamageView;}}
    
    public UIMiniMiniGame UIMiniMiniGame { get { return _uiMiniMiniGame;}}

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));

        _uiGameMenu = GetObject((int)Objects.UIGameMenu).GetOrAddComponent<UIGameMenu>();
        _uiPlayerInput = GetObject((int)Objects.UIMiniGameDeliveryPlayerInput).GetOrAddComponent<UIMiniGameDeliveryPlayerInput>();
        _uiPathProgressBar = GetObject((int)Objects.UIPathProgressBar).GetOrAddComponent<UIPathProgressBar>();
        _uiDamageView = GetObject((int)Objects.UIDamageView).GetOrAddComponent<UIDamageView>();
        _uiMiniMiniGame = GetObject((int)Objects.UIMiniMiniGame).GetOrAddComponent<UIMiniMiniGame>();
        
        return true;
    }
}
