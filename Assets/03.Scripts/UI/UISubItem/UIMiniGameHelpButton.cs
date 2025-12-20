using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class UIMiniGameHelpButton : UISubItem
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

        if(Managers.Stage.CurrentStageType >= Define.ChapterType.CH4)
        {
            gameObject.SetActive(false);
        }

        return true;
    }

    public void OnClickOptionButton(){
        if (Managers.MiniGame.CurrentGame.IsPause)
        {
            return;
        }
        
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Logger.Log("OnClickOption");

        if (Managers.MiniGame.CurrentGame is MiniGameUnload)
        {
            string str = "UIMGUTutorialPopup_" + Managers.Stage.CurrentStageType.ToString();
            Managers.UI.ShowPopUI<UITutorialPopup>(str);
        }
    }
}
