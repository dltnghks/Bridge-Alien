using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIHouseScene : UIScene
{
    enum Texts{
        GoldText,
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
        FatigueIconGroup
    }

    private UIPopup _currentPopup = null;
    private UIFatigueIconGroup _fatigueIconGroup;
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

        _fatigueIconGroup = GetObject((int)Objects.FatigueIconGroup).GetOrAddComponent<UIFatigueIconGroup>();
        
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
        var stagePopup = Managers.UI.ShowPopUI<UIStagePopup>();
        stagePopup.InitStageButtonGroup();
    }
    
    private void SetGoldText()
    {
        GetText((int)Texts.GoldText).text = $"{Managers.Player.GetGold()}N";
    }
    
    private void SetFatigue()
    {
        int curFatigue = Managers.Player.GetStats(Define.PlayerStatsType.Fatigue);
        _fatigueIconGroup.SetFatigue(curFatigue);
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
