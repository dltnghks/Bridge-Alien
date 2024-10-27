using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        SceneType = Define.Scene.GameMap;

        Managers.UI.ShowSceneUI<UIGameMapScene>();
        
        return true;
    }
}
