using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class HouseBedObject : MonoBehaviour, IInteractable
{
    public int Priority => 0;

    [Header("피로도 회복 시간 (분)")]
    [SerializeField]
    private float _totalRecoveryTime = 30f;
    private float _restTime = 5f;
    
    
    [Header("UI 요소")]
    [SerializeField] private TextMeshPro _restRecoveryTimeText;

    [Header("상호작용 이미지")]
    [SerializeField] private GameObject _timerBackground;
    [SerializeField] private GameObject _clickIcon;
    [SerializeField] private GameObject _inUseIcon;

    private bool _isInteract = false;
    public void Start()
    {
        _isInteract = false;
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

        if (!_isInteract)
        {
            return false;
        }

        _isInteract = false;
        return true;
    }

    public string GetInteractionPrompt()
    {
        return "휴식하기";
    }

    public void Interact(Interactor interactor)
    {

        HousePlayer player = interactor.GetComponent<HousePlayer>();
        Logger.Log("OnClickRestButton");
        if (!CanInteract())
        {
            interactor.InteractionComplete();
            return;
        }

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());

        // Start the interaction sequence as a coroutine
        StartCoroutine(PerformInteractionSequence(player, interactor));
    }

    private IEnumerator PerformInteractionSequence(HousePlayer player, Interactor interactor)
    {
        float arrivalThreshold = 1f;

        while (Vector3.Distance(player.transform.position, transform.position) > arrivalThreshold)
        {
            player.MoveToTarget(transform);
            yield return null; 
        }

        // Player has reached the target position, perform actions
        player.Rest(true);
        SetInUseIcon();

        // Initiate the delayed actions (5 seconds later) using DOTween
        DOVirtual.DelayedCall(_restTime, () =>
        {
            player.Rest(false);
            EndInteract(interactor);
        });
    }

    private void EndInteract(Interactor interactor)
    {
        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, 1);
        Managers.Player.PlayerData.FatigueRecoveryTime = DateTime.Now + TimeSpan.FromMinutes(_totalRecoveryTime);

        StartCoroutine(StartTimer());
        
        interactor.InteractionComplete();
    }

    private IEnumerator StartTimer()
    {
        SetTimerBackground();
        DateTime targetTime = Managers.Player.PlayerData.FatigueRecoveryTime;
        while (DateTime.Now < targetTime)
        {
            TimeSpan remainingTimeSpan = targetTime - DateTime.Now;
            string remainingTime = string.Format("{0:D2}:{1:D2}", remainingTimeSpan.Minutes, remainingTimeSpan.Seconds);
            _restRecoveryTimeText.SetText(remainingTime);

            yield return new WaitForSeconds(1f);
        }

        EndTimer();
    }

    // 타이머가 종료되면 상호작용 아이콘 띄우기
    private void EndTimer()
    {
        SetClickIcon();
        _isInteract = true;
    }

    private void SetTimerBackground()
    {
        _timerBackground.SetActive(true);
        _clickIcon.SetActive(false);
        _inUseIcon.SetActive(false);
    }

    private void SetClickIcon()
    {
        _timerBackground.SetActive(false);
        _clickIcon.SetActive(true);
        _inUseIcon.SetActive(false);
    }

    private void SetInUseIcon()
    {
        _timerBackground.SetActive(false);
        _clickIcon.SetActive(false);
        _inUseIcon.SetActive(true);
    }
}