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

    enum GameObjects
    {
        MiniGameTypeGameObject,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));
        BindObject(typeof(GameObjects));
        
        GetButton((int)Buttons.HouseButton).gameObject.BindEvent(OnClickHouseButton);
        GetButton((int)Buttons.MiniGameStartButton).gameObject.BindEvent(OnClickMiniGameStart);

        Managers.Scene.SetSelectedSceneAction(SetMiniGameTypeText);

        return true;
    }

    public void SetMiniGameTypeText(string str){
        if (str == Define.Scene.Unknown.ToString())
        {
            GetObject((int)GameObjects.MiniGameTypeGameObject).gameObject.SetActive(false);
        }
        else
        {
            GetObject((int)GameObjects.MiniGameTypeGameObject).gameObject.SetActive(true);
            Logger.Log($"change selected Scene Type : {str}");
            GetText((int)Texts.MiniGameTypeText).SetText(str);
        }
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
