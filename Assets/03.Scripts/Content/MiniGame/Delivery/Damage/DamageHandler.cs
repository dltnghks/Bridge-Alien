using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DamageHandler : MonoBehaviour
{
    [field: SerializeField, Header("현재 손상률")] 
    public float DamageRate { get; private set; }
    
    public Action onFullDamageRate;

    private bool _onStartDamage = false;

    [SerializeField, Header("초 당 손상률")]
    private float _damageRatePerSecond = 0.02f;
    private Coroutine _dotCoroutine;
    
    public void Initialize(Action onFullAction)
    {
        DamageRate = .0f;
        
        onFullDamageRate = onFullAction;
        _onStartDamage = false;
        _dotCoroutine = null;

        _dotCoroutine = StartCoroutine(OnGiveDotDamage());
    }

    public void OnDamage()
    {
        AddDamageRate(0.25f);
    }
    
    private void AddDamageRate(float rate)
    {
        if (rate <= 0f || DamageRate >= 1.0f)
            return;

        DamageRate = Mathf.Min(DamageRate + rate, 1.0f);
        UpdateDamageRate();
    }
    
    private void UpdateDamageRate()
    {
        if (DamageRate >= 1.0f)
        {
            if (_dotCoroutine != null)
            {
                StopCoroutine(_dotCoroutine);
                _dotCoroutine = null;
            }

            _onStartDamage = false;
            
            onFullDamageRate?.Invoke();
        }
        else if (DamageRate is < 1.0f and > 0.75f)
        {
            // 2%와 3% 중에서 랜덤으로 데미지 선택
            _damageRatePerSecond = Random.Range(0.02f, 0.04f);
        }
    }

    private IEnumerator OnGiveDotDamage()
    {
        while (DamageRate < 1.0f)
        {
            yield return new WaitForSeconds(1f);
            AddDamageRate(_damageRatePerSecond);
            
            UpdateDamageRate();
        }
        
        _onStartDamage = false;
        _dotCoroutine = null;
        Debug.Log("코루틴이 조건에 만족하여 종료되었습니다. -> OnGiveDotDamage");
    }
}