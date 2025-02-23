using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryPlayerController : IPlayerController
{
    public Player Player { get; set; }
    public int InteractionActionNumber { get; set; }

    public MiniGameDeliveryPlayerController(){}
    public MiniGameDeliveryPlayerController(Player player){
        Init(player);;
    }

    public void Init(Player player)
    {
        Player = player;
    }
    
    public void InputJoyStick(Vector2 input)
    {
        if(Managers.MiniGame.CurrentGame.IsPause){
           return; 
        }
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
        if(Managers.MiniGame.CurrentGame.IsPause){
           return; 
        }

        Logger.Log("Jump");
    }

    public bool ChangeInteraction(int actionnNum)
    {
        throw new System.NotImplementedException();
    }
}
