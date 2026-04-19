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
        StrengthText,              // 근력
        LuckText,                  // 운빨


        // Detail
        ExperienceDescText,            // 작업 숙련
        GravityAdaptationDescText,     // 중력 적응
        StrengthDescText,              // 근력
        LuckDescText,                  // 운빨
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
        SetStrengthDescText();
        SetLuckDescText();
    }

    private void SetExperienceDescText()
    {
        int value = Managers.Player.GetExperienceStatsBonusPercent();
        string newText = "미니게임 종료 시 <color=#36E100>{0}%</color>의 추가 점수를 획득합니다.";
        GetText((int)Texts.ExperienceDescText).SetText(string.Format(newText, value));
    }

    private void SetGravityAdaptationDescText()
    {
        // TODO: 중력적응에 대한 효과 설명 수정, 미니게임에서 이동속도가 증가하는 것으로 수정
        int value = Managers.Player.GetGravityAdaptationBonusPercent();
        string newText = "작업 진행 시 이동속도가 <color=#36E100>{0}%</color> 향상됩니다.";
        GetText((int)Texts.GravityAdaptationDescText).SetText(string.Format(newText, value));
    }
    private void SetStrengthDescText()
    {
        int value = Managers.Player.GetStrengthBonusPercent();
        string newText = "상자 운반 시 이동속도가 <color=#36E100>{0}%</color> 상승합니다.";
        GetText((int)Texts.StrengthDescText).SetText(string.Format(newText, value));
    }
    private void SetLuckDescText()
    {
        int value = Managers.Player.GetLuckBonusPercent();
        string newText = "럭키 보너스 확률이 <color=#36E100>{0}%</color> 증가합니다.";
        GetText((int)Texts.LuckDescText).SetText(string.Format(newText, value));
    }
}


