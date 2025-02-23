using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : UIPopup
{
    enum GameObjects
    {
        SoundAllOption,
        SoundBGMOption,
        SoundSFXOption,
    }

    enum Buttons
    {
        ExitButton,
    }
    
    private Slider _allSlider;
    private Slider _bgmSlider;
    private Slider _sfxSlider;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));

        _allSlider = GetObject((int)GameObjects.SoundAllOption).GetComponent<Slider>();
        _bgmSlider = GetObject((int)GameObjects.SoundBGMOption).GetComponent<Slider>();
        _sfxSlider = GetObject((int)GameObjects.SoundSFXOption).GetComponent<Slider>();

        if (_allSlider != null)
        {
            _allSlider.maxValue = 100f;
            _allSlider.value = Managers.Sound.AllVolume;
        }
        
        if (_bgmSlider != null)
        {
            _bgmSlider.maxValue = 100f;
            _bgmSlider.value = Managers.Sound.BGMVolume;
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.maxValue = 100f;
            _sfxSlider.value = Managers.Sound.SFXVolume;
        }
        
        // 슬라이더 값 변경 이벤트 연결
        _allSlider.onValueChanged.AddListener(Managers.Sound.SetAllVolume);
        _bgmSlider.onValueChanged.AddListener(Managers.Sound.SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(Managers.Sound.SetSFXVolume);
        
        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);
        
        return true;
    }

    private void OnClickExitButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        ClosePopupUI();
    }


}
