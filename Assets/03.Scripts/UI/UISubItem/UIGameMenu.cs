using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameMenu : UISubItem
{
    enum Buttons{
        MenuButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        GetButton((int)Buttons.MenuButton).gameObject.BindEvent(OnClickOptionButton);

        return true;
    }

    public void OnClickOptionButton(){
        Logger.Log("OnClickOption");
        Managers.MiniGame.PauseGame();
        Managers.UI.ShowPopUI<UIGameMenuPopup>();
    }
}
