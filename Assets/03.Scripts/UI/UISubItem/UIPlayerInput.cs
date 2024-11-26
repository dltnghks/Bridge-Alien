using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIPlayerInput : UISubItem
{
    enum Objects
    {
        Joystick,
    }

    enum Buttons
    {
        InteractionButton,
    }
    
    protected Joystick _joystick;
    protected Button _interactionButton;
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        BindObject(typeof(Objects));
        BindButton(typeof(Buttons));
        
        _joystick = GetObject((int)Objects.Joystick).GetOrAddComponent<Joystick>();
        _interactionButton = GetButton((int)Buttons.InteractionButton);

        _interactionButton.gameObject.BindEvent(OnClickInteractionButton);
        
        return true;
    }

    public void OnJoyStick(InputValue _value){
        if (Managers.MiniGame.CurrentGame.IsActive)
        {
            Managers.MiniGame.CurrentGame.PlayerController.InputJoyStick(_joystick.Direction);
        }
    }

    protected  void OnClickInteractionButton()
    {
        Managers.MiniGame.CurrentGame.PlayerController.Interaction();
    }

    public void FixedUpdate()
    {
        if (Managers.MiniGame.CurrentGame.IsActive)
        {
            Managers.MiniGame.CurrentGame.PlayerController.InputJoyStick(_joystick.Direction);
        }
    }
}
