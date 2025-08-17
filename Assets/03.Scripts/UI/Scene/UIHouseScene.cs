using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHouseScene : UIScene
{
    enum Texts{
        DayText,
        GoldText,
        FatigueText,
    }

    enum Buttons
    {
        PlayerStatusButton,
        TaskButton,
        WorkModuleButton,
        UINextButton,
    }

    enum Objects
    {
        UIFatigue,
    }

    enum Images
    {
        TimeImage,    
    }

    private UIPopup _currentPopup = null;
    private Slider _fatigueSlider;

    [SerializeField] private Sprite[] _timeImages;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));
        BindImage(typeof(Images));

        _fatigueSlider = GetObject((int)Objects.UIFatigue).GetOrAddComponent<Slider>();

        SetDayText();
        SetTimeImage();

        GetButton((int)Buttons.PlayerStatusButton).gameObject.BindEvent(OnClickPlayerStatusButton);
        GetButton((int)Buttons.TaskButton).gameObject.BindEvent(OnClickTaskButton);
        GetButton((int)Buttons.WorkModuleButton).gameObject.BindEvent(OnClickWorkModuleButton);
        GetButton((int)Buttons.UINextButton).gameObject.BindEvent(OnClickNextButton);

        SetGoldText();
        SetFatigue();

        return true;
    }

    public override void UIUpdate()
    {
        base.UIUpdate();
        SetDayText();
        SetTimeImage();
    }

    private void OnClickPlayerStatusButton()
    {
        if (_currentPopup)
        {
            _currentPopup.ClosePopupUI();
            return;
        }
        
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _currentPopup = Managers.UI.ShowPopUI<UIPlayerStatusPopup>();
    }

    private void OnClickTaskButton()
    {
        // 일과 팝업 생성
        if (_currentPopup)
        {
            _currentPopup.ClosePopupUI();
            return;
        }

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        UIPlayerTaskPopup taskPopup = Managers.UI.ShowPopUI<UIPlayerTaskPopup>("UIPlayerTaskPopup", transform);
    }


    private void OnClickWorkModuleButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _currentPopup = Managers.UI.ShowPopUI<UIWorkModulePopup>();
    }

    private void OnClickNextButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.UI.ShowPopUI<UIStagePopup>();
        //Managers.Daily.StartEvent();
        //GetButton((int)Buttons.UINextButton).gameObject.SetActive(false);
    }

    private void SetDayText()
    {
        GetText((int)Texts.DayText).SetText($"Day {Managers.Daily.CurrentDate}");
    }

    private void SetTimeImage()
    {
        if (_timeImages.Length <= Managers.Daily.CurrentDailyData.Time)
        {
            return;
        }
        GetImage((int)Images.TimeImage).sprite = _timeImages[Managers.Daily.CurrentDailyData.Time]; 
    }
    
    private void SetGoldText()
    {
        GetText((int)Texts.GoldText).text = $"{Managers.Player.GetGold()}N";
    }
    
    private void SetFatigue()
    {
        int curFatigue = Managers.Player.GetStats(Define.PlayerStatsType.Fatigue);
        GetText((int)Texts.FatigueText).text = $"{curFatigue}/100";
        _fatigueSlider.DOValue((float)curFatigue / 100f, 0.5f);
    }

    private void OnEnable()
    {
        Managers.Player.OnPlayerDataChanged += SetGoldText;
        Managers.Player.OnPlayerDataChanged += SetFatigue;
    }
    
    private void OnDisable()
    {
        Managers.Player.OnPlayerDataChanged -= SetGoldText;
        Managers.Player.OnPlayerDataChanged -= SetFatigue;
    }
}
