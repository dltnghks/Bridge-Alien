using System.Collections.Generic;
using UnityEngine;

public class MiniGameDeliveryPlayer : Player
{
    private IHealthPoint _healthPoint;
    
    public void Start()
    {
        base.Start();
        _healthPoint = GetComponent<IHealthPoint>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            _healthPoint.TakeDamage(10f);
        }
    }
}