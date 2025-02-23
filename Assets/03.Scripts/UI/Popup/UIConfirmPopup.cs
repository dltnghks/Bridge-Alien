using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConfirmPopup : UIPopup
{
    enum Buttons
    {
        ConfirmButton,
    }
    
    enum Texts
    {
        ConfirmButtonText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        
        return true;
    }

    protected virtual void OnClickConfirmButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Logger.Log("OnClickConfirmButton");
    } 
}
