using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthPointComponent : MonoBehaviour
{
    [SerializeField] private float _currentHP;
    [SerializeField] private float _maxHP;

    private UnityAction<float, float> _changeHPAction;

    void SetHP(float maxHP, UnityAction<float, float> changeHPAction){
        _maxHP = maxHP;
        _currentHP = maxHP;
        _changeHPAction = changeHPAction;
        _changeHPAction?.Invoke(_maxHP, _currentHP);
    }
    void AddHP(float value){
        _currentHP = Mathf.Clamp(_currentHP+value, 0, _maxHP);
        _changeHPAction?.Invoke(_maxHP, _currentHP);

        if(_currentHP <= 0){
            Death();
        }

    }
    void Death(){
        Logger.Log("Death");
    }
}
