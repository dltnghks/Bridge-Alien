using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private Joystick _joystick;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(Objects));
        BindButton(typeof(Buttons));
        
        _joystick = GetObject((int)Objects.Joystick).GetOrAddComponent<Joystick>();
        GetButton((int)Buttons.InteractionButton).gameObject.BindEvent(OnClickInteractionButton);
        Debug.Log(_joystick.Input);
        
        return true;
    }

    private void OnClickInteractionButton()
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
