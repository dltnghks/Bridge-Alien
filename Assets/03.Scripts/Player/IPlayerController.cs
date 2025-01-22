using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPlayerController
{
    Player Player { get; set; }

    public int InteractionActionNumber { get; set; }

    public void Init(Player player);
    public void InputJoyStick(Vector2 input);
    public void Interaction();
    public bool ChangeInteraction(int actionNum);
}
