using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageHandler : MonoBehaviour
{
    public float DamageRate
    {
        get
        {
            return _damageRate;
        }
        private set
        {
            _damageRate = Mathf.Clamp01(value);
            // 손상률 데이터 콜백
            OnDamageRateChanged(_damageRate);
            UpdateSpeedPenalty();

            if (_damageRate >= 1.0f)
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
    [SerializeField] private float speedPenalty10 = 0.6f;
    [SerializeField] private float speedPenalty20 = 0.4f;

    public float SpeedPenalty { get; private set; } = 1f;

    public Action OnFullDamage; // 손상률 100% 도달 시 콜백

    public Action<float> OnDamageRateChanged;

    public void Initialize(Action onFullAction)
    {
        OnFullDamage = onFullAction;

        _damageRate = 0f;
        DamageRate = 0f;
        SpeedPenalty = 1f;
        _onStartDamage = false;

        // 만약에 dotCoroutine이 돌고 있다면.
        if (_dotCoroutine != null)
        {
            StopCoroutine(_dotCoroutine);
        }
    }

    public void OnResetDamage(bool isOn)
    {
        DamageRate -= Mathf.Clamp01(0.25f);
        UpdateSpeedPenalty();
    }

    public void OnClearDamage()
    {
        DamageRate = 0f;
        UpdateSpeedPenalty();
    }

    public void OnDamage()
    {
        DamageRate += hurdleDamage;

        // 초 당 손상률을 당하고 있지 않는다.
        // 데미지가 75% 이상 100% 미만이다.
        if (!_onStartDamage && DamageRate >= 0.75f && DamageRate < 1.0f)
        {
            _dotCoroutine = StartCoroutine(OnGiveDotDamage());
            _onStartDamage = true;
        }
    }

    private void UpdateSpeedPenalty()
    {
        if (DamageRate >= 0.86f && DamageRate < 1.0f)
        {
            SpeedPenalty = speedPenalty20;
        }
        else if (DamageRate >= 0.75f && DamageRate <= 0.85f)
        {
            SpeedPenalty = speedPenalty10;
        }
        else
        {
            SpeedPenalty = 1f;
        }
    }

    private IEnumerator OnGiveDotDamage()
    {
        // 손상률이 75 ~ 100% 사이일 때.
        while (DamageRate >= 0.75f && DamageRate < 1.0f)
        {
            yield return new WaitForSeconds(1f);
            DamageRate += Random.Range(damageMin, damageMax);
        }

        _onStartDamage = false;
        _dotCoroutine = null;
    }

    // 손상률이 100%에 도달했을 때, 발동한다.
    private void TriggerFullDamage()
    {
        DamageRate = 1.0f;

        if (_dotCoroutine != null)
        {
            StopCoroutine(_dotCoroutine);
            _dotCoroutine = null;
        }

        _onStartDamage = false;
        OnFullDamage?.Invoke();
    }
}
