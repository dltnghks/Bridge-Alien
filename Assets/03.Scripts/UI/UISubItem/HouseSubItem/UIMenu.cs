using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : UISubItem
{
    enum Buttons
    {
        MapButton,
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.MapButton).gameObject.BindEvent(OnClickMapButton);

        return true;
    }

    private void OnClickMapButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Scene.ChangeScene(Define.Scene.GameMap);
    }
}
