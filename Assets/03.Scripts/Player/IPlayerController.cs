using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController
{
    Player Player { get; set; }
    public void InputJoyStick(Vector2 input);
    public void Interaction();
}
