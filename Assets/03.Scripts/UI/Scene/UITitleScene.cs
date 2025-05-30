using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UITitleScene : UIScene
{
    enum Buttons
    {
        GameStartButton,
        LoadButton,
    }

    enum Images
    {
        
    }

    enum Texts
    {
        
    }

    enum Objects
    {
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindText(typeof(Texts));
        BindObject(typeof(Objects));
        
        
        GetButton((int)Buttons.GameStartButton).gameObject.BindEvent(OnClickStartButton);
        GetButton((int)Buttons.LoadButton).gameObject.BindEvent(OnClickLoadButton);

        return true;
    }
    
    public void OnClickStartButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Scene.ChangeScene(Define.Scene.House);

    }

    private void OnClickLoadButton()
    {
        Managers.Save.Load();
        Managers.Scene.ChangeScene(Define.Scene.House);
    }
}
