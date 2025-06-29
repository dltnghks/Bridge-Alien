using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameMenuPopup : UIPopup
{
    enum Buttons{
        ResumeButton,
        OptionButton,
        ExitButton,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.ResumeButton).gameObject.BindEvent(OnClickResumeButton);
        GetButton((int)Buttons.OptionButton).gameObject.BindEvent(OnClickOptionButton);
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);

        return true;
    }

    private void OnClickResumeButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Logger.Log("OnClickResume");
        Managers.MiniGame.ResumeGame();
        Managers.UI.ClosePopupUI(this);
    }

    private void OnClickOptionButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Logger.Log("OnClickOption");
        Managers.UI.ShowPopUI<UIOption>();
    }

    private void OnClickExitButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Logger.Log("OnClickExit");
    }
}
