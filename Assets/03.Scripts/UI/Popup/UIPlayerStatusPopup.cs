using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatusPopup : UIPopup
{
    enum Texts
    {
        // Status
        ExperienceText,            // 작업 숙련
        GravityAdaptationText,     // 중력 적응
        IntelligenceText,          // 지능
        LuckText,                  // 운


        // Detail
        ExperienceDescText,            // 작업 숙련
        GravityAdaptationDescText,     // 중력 적응
        IntelligenceDescText,          // 지능
        LuckDescText,                  // 운
    }

    enum Buttons
    {
        DetailButton,
    }

    enum Objects
    {
        Status,
        Detail,
        Status_PlayerStatusGroup,
        Detail_PlayerStatusGroup,
    }

    private GameObject _detail;
    private UIStatusTextGroup _status_PlayerStatusTextGroup;
    private UIStatusTextGroup _detail_PlayerStatusTextGroup;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        GetButton((int)Buttons.DetailButton).gameObject.BindEvent(OnPointDownDetailButtonButton, Define.UIEvent.PointerDown);
        GetButton((int)Buttons.DetailButton).gameObject.BindEvent(OnPointUpdDetailButtonButton, Define.UIEvent.PointerUp);

        _detail = GetObject((int)Objects.Detail);

        _status_PlayerStatusTextGroup = GetObject((int)Objects.Status_PlayerStatusGroup).GetOrAddComponent<UIStatusTextGroup>();
        _detail_PlayerStatusTextGroup = GetObject((int)Objects.Detail_PlayerStatusGroup).GetOrAddComponent<UIStatusTextGroup>();

        UpdateUI();

        _detail.SetActive(false);

        return true;
    }

    public void UpdateUI()
    {
        _status_PlayerStatusTextGroup.SetPlayerStat();
        _detail_PlayerStatusTextGroup.SetPlayerStat();

        SetDetailStatusDescText();
    }

    private void OnPointDownDetailButtonButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _detail.SetActive(true);
    }

    private void OnPointUpdDetailButtonButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        _detail.SetActive(false);
    }

    private void SetDetailStatusDescText()
    {
        SetExperienceDescText();
        SetGravityAdaptationDescText();
        SetIntelligenceDescText();
        SetLuckDescText();
    }

    private void SetExperienceDescText()
    {
        int value = (int)(Managers.Player.GetExperienceStatsBonus() * 100);
        string newText = "미니게임 종료 시 획득하는 골드가 <color=#00FF00>{0}%</color>증가한다.";
        GetText((int)Texts.ExperienceDescText).SetText(string.Format(newText, value));
    }

    private void SetGravityAdaptationDescText()
    {
        int value = Managers.Player.GetFatigueReductionRate();
        string newText = "미니게임 시 소모되는 피로도가 <color=#00FF00>{0}</color>으로 조정된다.";
        GetText((int)Texts.GravityAdaptationDescText).SetText(string.Format(newText, value));
    }
    private void SetIntelligenceDescText()
    {
        string newText = "총명한 자는 선택지가 많아진다.";
        GetText((int)Texts.IntelligenceDescText).SetText(newText);
    }
    private void SetLuckDescText()
    {
        string newText = "운이 좋다면 특별한 일이 생길지도?";
        GetText((int)Texts.LuckDescText).SetText(newText);
    }
}
