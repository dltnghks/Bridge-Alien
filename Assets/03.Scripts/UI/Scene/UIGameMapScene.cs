using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameMapScene : UIScene
{
    enum Buttons
    {
        HouseButton,   
        GameUnloadButton,
        
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.HouseButton).gameObject.BindEvent(OnClickHouseButton);
        GetButton((int)Buttons.GameUnloadButton).gameObject.BindEvent(OnClickGameUnloadButton);
        
        return true;
    }

    private void OnClickHouseButton()
    {
        Managers.Scene.ChangeScene(Define.Scene.House);
    }

    private void OnClickGameUnloadButton()
    {
        Managers.Scene.ChangeScene(Define.Scene.GameUnload);
    }
}
