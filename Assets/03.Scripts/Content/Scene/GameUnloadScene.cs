using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUnloadScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.MiniGameUnload;
        
        Managers.MiniGame.LoadStage();
        return true;
    }
}

