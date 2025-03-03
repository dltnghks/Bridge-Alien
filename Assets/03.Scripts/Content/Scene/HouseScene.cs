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

        Managers.UI.ShowSceneUI<UIHouseScene>();
        
        // 테스트 코드
        Managers.Daily.AddDate();
        return true;
    }
}
