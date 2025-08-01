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
    }
    
    private UIMiniGameHelpButton _uiGameMenu;
    private UIMiniGameDeliveryPlayerInput _uiPlayerInput;
    private UIPathProgressBar _uiPathProgressBar;
    
    public UIMiniGameDeliveryPlayerInput UIPlayerInput { get { return _uiPlayerInput; } }
    public UIPathProgressBar UIPathProgressBar { get { return _uiPathProgressBar;}}

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));

        _uiGameMenu = GetObject((int)Objects.UIGameMenu).GetOrAddComponent<UIMiniGameHelpButton>();
        _uiPlayerInput = GetObject((int)Objects.UIMiniGameDeliveryPlayerInput).GetOrAddComponent<UIMiniGameDeliveryPlayerInput>();
        _uiPathProgressBar = GetObject((int)Objects.UIPathProgressBar).GetOrAddComponent<UIPathProgressBar>();
        
        return true;
    }
}
