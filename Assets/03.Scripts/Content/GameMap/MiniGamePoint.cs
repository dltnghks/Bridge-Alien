using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePoint : ClickableObject
{
    public Define.Scene MiniGameType = Define.Scene.Unknown;

    public override void OnClick()
    {
        Debug.Log($"{name} clicked!");
    }
}
