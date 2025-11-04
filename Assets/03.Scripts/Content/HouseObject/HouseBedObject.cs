using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class HouseBedObject : MonoBehaviour, IHouseInteractiveObject
{
    [SerializeField]
    private TextMeshPro _restRecoveryTimeText;

    public void Start()
    {
        // 타이머 동작
        StartCoroutine(StartTimer());
        
        _restRecoveryTimeText.SetText("휴식하기");
    }
    
    public bool CanInteract()
    {
        if(Managers.Player.PlayerData.FatigueRecoveryTime > DateTime.Now ||
            Managers.Player.GetStats(Define.PlayerStatsType.Fatigue) >= 3)
        {
            return false;
        }
        return true;
    }

    public void Interact(HousePlayer player)
    {
        // 플레이어 재우기
        // 피로도 회복
        Logger.Log("OnClickRestButton");
        if (!CanInteract()) 
            return;

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, 1);
        Managers.Player.PlayerData.FatigueRecoveryTime = DateTime.Now + TimeSpan.FromMinutes(30);
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        DateTime targetTime = Managers.Player.PlayerData.FatigueRecoveryTime;
        while (DateTime.Now < targetTime)
        {
            // mm:ss 형식으로 남은 시간 계산
            TimeSpan remainingTimeSpan = targetTime - DateTime.Now;
            string remainingTime = string.Format("휴식하기\n{0:D2}:{1:D2}", remainingTimeSpan.Minutes, remainingTimeSpan.Seconds);

            // 텍스트 업데이트
            _restRecoveryTimeText.SetText(remainingTime);

            // 1초 대기
            yield return new WaitForSeconds(1f);
        }
        
        // 타이머가 종료되면 텍스트 초기화
        _restRecoveryTimeText.SetText("휴식하기");
    }

}