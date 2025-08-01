using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    enum Texts
    {
        SoundAllText,
        SoundBGMText,
        SoundSFXText,
    }

    private Slider _allSlider;
    private Slider _bgmSlider;
    private Slider _sfxSlider;

    private TextMeshProUGUI _allText;
    private TextMeshProUGUI _bgmText;
    private TextMeshProUGUI _sfxText;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindObject(typeof(GameObjects));
        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        _allSlider = GetObject((int)GameObjects.SoundAllOption).GetComponent<Slider>();
        _bgmSlider = GetObject((int)GameObjects.SoundBGMOption).GetComponent<Slider>();
        _sfxSlider = GetObject((int)GameObjects.SoundSFXOption).GetComponent<Slider>();

        _allText = GetText((int)Texts.SoundAllText);
        _bgmText = GetText((int)Texts.SoundBGMText);
        _sfxText = GetText((int)Texts.SoundSFXText);

        if (_allSlider != null)
        {
            _allSlider.maxValue = 100f;
            _allSlider.value = Managers.Sound.AllVolume;
            SetSoundAllText(Managers.Sound.AllVolume);
        }

        if (_bgmSlider != null)
        {
            _bgmSlider.maxValue = 100f;
            _bgmSlider.value = Managers.Sound.BGMVolume;
            SetSoundBgmText(Managers.Sound.BGMVolume);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.maxValue = 100f;
            _sfxSlider.value = Managers.Sound.SFXVolume;
            SetSoundSfxText(Managers.Sound.SFXVolume);
        }

        // 슬라이더 값 변경 이벤트 연결
        _allSlider.onValueChanged.AddListener(Managers.Sound.SetAllVolume);
        _bgmSlider.onValueChanged.AddListener(Managers.Sound.SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(Managers.Sound.SetSFXVolume);

        // 슬라이더 값 변경 이벤트 연결
        _allSlider.onValueChanged.AddListener(SetSoundAllText);
        _bgmSlider.onValueChanged.AddListener(SetSoundBgmText);
        _sfxSlider.onValueChanged.AddListener(SetSoundSfxText);

        GetButton((int)Buttons.ExitButton).gameObject.BindEvent(OnClickExitButton);

        return true;
    }

    private void OnClickExitButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        ClosePopupUI();
    }

    private void SetSoundAllText(float value)
    {
        _allText.SetText($"전체 {value:0}");
    }

    private void SetSoundBgmText(float value)
    {
        _bgmText.SetText($"배경음 {value:0}");
    }
    private void SetSoundSfxText(float value)
    {
        _sfxText.SetText($"효과음 {value:0}");
    }

}
