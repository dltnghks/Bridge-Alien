using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UITitleScene : UIScene
{
    enum Buttons
    {
        GameStartButton,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent(OnClickStartButton);

        return true;
    }
    
    public void OnClickStartButton()
    {
        Managers.Scene.ChangeScene(Define.Scene.House);

    }
}
