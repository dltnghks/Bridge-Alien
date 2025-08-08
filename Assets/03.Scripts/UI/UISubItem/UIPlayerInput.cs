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
    private Vector2 _movementInput;

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
        
        return true;
    }
    
    public void OnMove(InputValue value){
        _movementInput = value.Get<Vector2>();
    }

    public void OnInteraction(){
        Managers.MiniGame.CurrentGame.PlayerController.Interaction();
    }

    private void Update(){
        if(Managers.MiniGame.CurrentGame.IsActive)
            Managers.MiniGame.CurrentGame.PlayerController.InputJoyStick(_movementInput);
    }
}
