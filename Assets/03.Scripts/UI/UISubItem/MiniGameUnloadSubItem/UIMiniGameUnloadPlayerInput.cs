using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;

public class UIMiniGameUnloadPlayerInput : UIPlayerInput
{
    enum Images
    {
        InteractionButtonImage,
    }
    
    [Header("Interaction Sprites")]
    [SerializeField] private List<Sprite> _spriteList = new List<Sprite>();
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        _init = true;
        
        BindImage(typeof(Images));
        
        return _init;
    }

    public void SetInteractionButtonSprite()
    {
        if(_init)
        {
            int num = Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber;
            GetImage((int)Images.InteractionButtonImage).sprite = _spriteList[num];
        }
    }
}
