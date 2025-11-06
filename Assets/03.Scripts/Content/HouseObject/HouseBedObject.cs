using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class HouseBedObject : MonoBehaviour, IHouseInteractiveObject
{
    [Header("피로도 회복 시간 (분)")]
    [SerializeField]
    private float _totalRecoveryTime = 30f;
    
    
    [Header("UI 요소")]
    [SerializeField]
    private TextMeshPro _restRecoveryTimeText;

    [SerializeField]
    private SpriteRenderer _progressBarFill;

    public void Start()
    {
        // 타이머 동작
        StartCoroutine(StartTimer());

    }

    public bool CanInteract()
    {
        if (Managers.Player.PlayerData.FatigueRecoveryTime > DateTime.Now ||
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
        Managers.Player.PlayerData.FatigueRecoveryTime = DateTime.Now + TimeSpan.FromMinutes(_totalRecoveryTime);
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        DateTime targetTime = Managers.Player.PlayerData.FatigueRecoveryTime;
        while (DateTime.Now < targetTime)
        {
            // mm:ss 형식으로 남은 시간 계산
            TimeSpan remainingTimeSpan = targetTime - DateTime.Now;
            string remainingTime = string.Format("{0:D2}:{1:D2}", remainingTimeSpan.Minutes, remainingTimeSpan.Seconds);

            // 텍스트 업데이트
            _restRecoveryTimeText.SetText(remainingTime);

            // 진행 바 업데이트
            float totalSeconds = (float)(targetTime - (targetTime - TimeSpan.FromMinutes(_totalRecoveryTime))).TotalSeconds;
            float remainingSeconds = (float)remainingTimeSpan.TotalSeconds;
            float fillAmount = 1f - (remainingSeconds / totalSeconds);
            _progressBarFill.material.SetFloat("_FillAmount", fillAmount);

            // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        // 타이머가 종료되면 텍스트 초기화
        _restRecoveryTimeText.SetText("휴식하기");
    }
}