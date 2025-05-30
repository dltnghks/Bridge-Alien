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
        
        // 집으로 들어오는 경우, 바로 이벤트 진행 
        Managers.Daily.StartEvent();
        
        return true;
    }
}
