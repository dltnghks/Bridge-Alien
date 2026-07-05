using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        SceneType = Define.Scene.Ending;
        Managers.UI.ShowSceneUI<UIEndingScene>();

        return true;
    }
}
