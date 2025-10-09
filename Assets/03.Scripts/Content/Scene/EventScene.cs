using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.EventScene;

        Managers.UI.ShowSceneUI<UIEventScene>();
        Managers.Event.PlayEvent();
        return true;
    }
}
