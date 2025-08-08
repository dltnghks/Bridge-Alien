using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIGameMenuPopup : UIPopup
{
    enum Buttons
    {
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

        Managers.MiniGame.PauseGame();

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
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 게임 종료
#endif
    }

    private void OnDestroy()
    {
        Managers.DeviceInput.IsMenuPopup = false;
        Managers.MiniGame.ResumeGame();
    }
}
