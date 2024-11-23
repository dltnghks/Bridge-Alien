using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameUnloadPlayerController : IPlayerController
{
    public Player Player { get; set; }
    
    public void InputJoyStick(Vector2 input)
    {
        // 플레이어 이동
        Player.PlayerMovement(input);
    }

    public void Interaction()
    {
        // 상자 들기
        PickupBox();
        
        // 상자 내리기
    }

    private void PickupBox()
    {
        Debug.Log("Pickup box");
        
    }

    private void DropBox()
    {
        Debug.Log("Drop box");
    }
    
}
