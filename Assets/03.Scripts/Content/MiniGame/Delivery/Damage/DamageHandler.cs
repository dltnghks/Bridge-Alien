using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class DamageHandler : MonoBehaviour
{
    public float DamageRate
    {
        get => _damageRate;
        private set
        {
            _damageRate = Mathf.Clamp01(value);
            OnDamageRateChanged?.Invoke(_damageRate);
            UpdateSpeedPenalty();

            if (_damageRate <= 0f)
                TriggerFullDamage();
        }
    }

    private float _damageRate;
    private bool _onStartDamage;
    private Coroutine _dotCoroutine;

    [Header("손상률 조절 수치")]
    [SerializeField] private float hurdleDamage = 0.25f;
    [SerializeField] private float damageMin = 0.02f;
    [SerializeField] private float damageMax = 0.03f;

    [Header("이동 속도 관련")]
    [SerializeField] private float speedPenalty25 = 0.8f;
    [SerializeField] private float speedPenalty50 = 0.6f;

    public float SpeedPenalty { get; private set; } = 1f;

    public Action OnFullDamage;
    public Action<float> OnDamageRateChanged;

    public void Initialize(Action onFullAction)
    {
        OnFullDamage = onFullAction;
        _damageRate = 1f;
        DamageRate = 1f;
        SpeedPenalty = 1f;
        _onStartDamage = false;

        if (_dotCoroutine != null)
            StopCoroutine(_dotCoroutine);
    }

    public void OnResetDamage(bool isOn)
    {
        if (isOn)
        {
            DamageRate -= Mathf.Clamp01(0.25f);
            UpdateSpeedPenalty();
        }
    }

    public void OnDamage()
    {
        DamageRate -= hurdleDamage;

        if (!_onStartDamage && DamageRate <= 0.25f && DamageRate > 0f)
        {
            _dotCoroutine = StartCoroutine(OnGiveDotDamage());
            _onStartDamage = true;
        }
    }

    private void UpdateSpeedPenalty()
    {
        if (DamageRate <= 0.25f && DamageRate > 0f)
        {
            SpeedPenalty = speedPenalty25;
        }
        else if (DamageRate <= 0.5f && DamageRate > 0.25f)
        {
            SpeedPenalty = speedPenalty50;
        }
        else
        {
            SpeedPenalty = 1f;
        }
    }

    private IEnumerator OnGiveDotDamage()
    {
        while (DamageRate <= 0.25f && DamageRate > 0f)
        {
            yield return new WaitForSeconds(1f);
            DamageRate -= Random.Range(damageMin, damageMax);
        }

        _onStartDamage = false;
        _dotCoroutine = null;
    }

    private void TriggerFullDamage()
    {
        _damageRate = 0f;

        if (_dotCoroutine != null)
        {
            StopCoroutine(_dotCoroutine);
            _dotCoroutine = null;
        }
        
        _onStartDamage = false;
        
        OnFullDamage?.Invoke();
        
        // 미니 게임 실행
        // ((UIGameDeliveryScene)Managers.MiniGame.CurrentGame.GameUI).UIMiniMiniGame.StartMiniGame();
        var miniminiGame = Managers.UI.ShowPopUI<UIMiniMiniGame>();
        
        miniminiGame.onRepairEvent?.AddListener(OnFinishRepairGame);
    }

    private void OnFinishRepairGame(bool isFlag)
    {
        if (isFlag)     // 수리 성공
        {
            _damageRate = 1f;
        }
        else            // 수리 실패
        {
            _damageRate = 0.5f;
        }
        
        UpdateSpeedPenalty();
        ((MiniGameDeliveryPlayer)Managers.MiniGame.CurrentGame.PlayerCharacter).StartInvincible(2f);
    }
}
