using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class UIRestButton : UISubItem
{

    enum Buttons
    {
        UIRestButton
    }

    enum Texts
    {
        RestRecoveryTimeText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindText(typeof(Texts));

        GetButton((int)Buttons.UIRestButton).gameObject.BindEvent(OnClickRestButton);

        StartCoroutine(StartTimer());

        return true;
    }

    private IEnumerator StartTimer()
    {
        DateTime targetTime = Managers.Player.PlayerData.FatigueRecoveryTime;
        while (DateTime.Now < targetTime)
        {
            // mm:ss 형식으로 남은 시간 계산
            TimeSpan remainingTimeSpan = targetTime - DateTime.Now;
            string remainingTime = string.Format("휴식하기\n{0:D2}:{1:D2}", remainingTimeSpan.Minutes, remainingTimeSpan.Seconds);

            GetText((int)Texts.RestRecoveryTimeText).SetText(remainingTime);
            // 1초 대기
            yield return new WaitForSeconds(1f);
        }
        
        GetText((int)Texts.RestRecoveryTimeText).SetText("휴식하기");
    }

    public void OnClickRestButton()
    {
        Logger.Log("OnClickRestButton");
        if (Managers.Player.PlayerData.FatigueRecoveryTime > DateTime.Now ||
            Managers.Player.GetStats(Define.PlayerStatsType.Fatigue) >= 3
        ) 
            return;

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, 1);
        Managers.Player.PlayerData.FatigueRecoveryTime = DateTime.Now + TimeSpan.FromMinutes(30);
        StartCoroutine(StartTimer());
    }
}
