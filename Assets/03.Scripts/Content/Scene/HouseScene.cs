using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Dev;

        Managers.UI.ShowSceneUI<UITitleScene>();
        return true;
    }
}
