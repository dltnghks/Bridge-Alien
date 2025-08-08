using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIDeliveryInteraction : UISubItem
{
    enum Buttons
    {
        InteractionButton,
    }

    private Action _onClickAction;
    private Action _onReleaseAction;
    
    public override bool Init()
    {
        if(base.Init() == false){
            return false;
        }

        BindButton(typeof(Buttons));

        var interactionButton = GetButton((int)Buttons.InteractionButton);
        BindEvent(interactionButton.gameObject, OnClicksInteractionButton, Define.UIEvent.PointerDown);
        BindEvent(interactionButton.gameObject, OnReleaseInteractionButton, Define.UIEvent.PointerUp);
        
        return true;
    }

    public void SetUp(Action onClickAction, Action onReleaseAction)
    {
        _onClickAction += onClickAction;
        _onReleaseAction += onReleaseAction;
    }

    private void OnClicksInteractionButton()
    {
        _onClickAction?.Invoke();
    }

    private void OnReleaseInteractionButton()
    {
        _onReleaseAction?.Invoke();
    }
}
