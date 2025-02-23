using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDeliveryScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.MiniGameDelivery;
        
        Managers.MiniGame.SelectMiniGame(Define.MiniGameType.Delivery);
        return true;
    }
}
