using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 1차 시연용 팝업 클래스
public class UIMiniGameChoicePopup : UIPopup
{
    enum Buttons
    {
        UnloadGameButton,
        DeliveryGameButton
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));

        GetButton((int)Buttons.UnloadGameButton).gameObject.BindEvent(OnClickUnloadGameButton);
        GetButton((int)Buttons.DeliveryGameButton).gameObject.BindEvent(OnClickDeliveryGameButton);

        return true;
    }

    public void OnClickUnloadGameButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload);
        ClosePopupUI();
    }

    public void OnClickDeliveryGameButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Scene.ChangeScene(Define.Scene.MiniGameDelivery);
        ClosePopupUI();
    }


}
