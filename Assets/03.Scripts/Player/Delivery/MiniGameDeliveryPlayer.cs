using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryPlayer : Player
{
    private IHealthPoint _healthPoint;
    
    public UIHPGauge healthGauge;

    public void Start()
    {
        base.Start();
        _healthPoint = GetComponent<IHealthPoint>();

        if (_healthPoint != null)
            _healthPoint.OnHealthChanged += healthGauge.SetHP;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _healthPoint.TakeDamage(10f);
        }
    }
}