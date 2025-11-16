using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HouseBedObject : MonoBehaviour, IHouseInteractiveObject
{
    [Header("피로도 회복 시간 (분)")]
    [SerializeField]
    private float _totalRecoveryTime = 30f;
    
    
    [Header("UI 요소")]
    [SerializeField] private TextMeshPro _restRecoveryTimeText;

    [Header("상호작용 이미지")]
    [SerializeField] private SpriteRenderer _progressBarFill;
    [SerializeField] private SpriteRenderer _clickIcon;
    [SerializeField] private SpriteRenderer _inUseIcon;


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
        Logger.Log("OnClickRestButton");
        if (!CanInteract())
            return;

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());

        // 클릭 시 플레이어 이동 후 애니메이션, 아이콘 변경
        player.MoveToTarget(transform);
        player.Rest(true);
        // 애니메이션이 종료되면 피로도 회복 및 타이머 진행

        //DOTween

        EndInteract();
    }

    private void EndInteract()
    {
        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, 1);
        Managers.Player.PlayerData.FatigueRecoveryTime = DateTime.Now + TimeSpan.FromMinutes(_totalRecoveryTime);

        SetInUseIcon();
        player.Rest(false);

        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        SetActiveProgressBar();
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

        EndTimer();
    }

    // 타이머가 종료되면 상호작용 아이콘 띄우기
    private void EndTimer()
    {
        SetClickIcon();
        _restRecoveryTimeText.SetText("휴식하기");
    }

    private void SetActiveProgressBar()
    {
        _progressBarFill.gameObject.SetActive(true);
        _clickIcon.gameObject.SetActive(false);
        _inUseIcon.gameObject.SetActive(false);
    }

    private void SetClickIcon()
    {
        _progressBarFill.gameObject.SetActive(false);
        _clickIcon.gameObject.SetActive(true);
        _inUseIcon.gameObject.SetActive(false);
    }

    private void SetInUseIcon()
    {
        _progressBarFill.gameObject.SetActive(false);
        _clickIcon.gameObject.SetActive(false);
        _inUseIcon.gameObject.SetActive(true);
    }
}