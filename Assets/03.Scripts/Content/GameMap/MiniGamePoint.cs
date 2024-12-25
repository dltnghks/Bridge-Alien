using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGamePoint : ClickableObject
{
    public Define.Scene MiniGameSceneType = Define.Scene.Unknown;

    public override void OnClick()
    {
        Debug.Log($"{name} clicked!");
        Managers.Scene.SelectedSceneType = MiniGameSceneType;
    }
}
