using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameMapScene : UIScene
{
    enum Buttons
    {
        HouseButton,   
        //GameUnloadButton,
        MiniGameStartButton,
    }

    enum Texts
    {
        MiniGameTypeText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        
        GetButton((int)Buttons.HouseButton).gameObject.BindEvent(OnClickHouseButton);
        GetButton((int)Buttons.MiniGameStartButton).gameObject.BindEvent(OnClickMiniGameStart);
        
        Managers.Scene.SelectedSceneAction += SetMiniGameTypeText;

        return true;
    }

    public void SetMiniGameTypeText(string str){
        Logger.Log("change selected Scene Type");
        GetText((int)Texts.MiniGameTypeText).SetText(str);
    }

    private void OnClickHouseButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Scene.ChangeScene(Define.Scene.House);
    }

    private void OnClickMiniGameStart()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Scene.ChangeSelectedScene();

    }
}
