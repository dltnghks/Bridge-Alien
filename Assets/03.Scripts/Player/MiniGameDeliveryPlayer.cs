using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class MiniGameDeliveryPlayer : Player
{
    private IHealthPoint healthPoint;
    
    public UIHPGauge healthGauge;

    public void Start()
    {
        base.Start();
        healthPoint = GetComponent<IHealthPoint>();

        if (healthPoint != null)
        {
            healthPoint.OnHealthChanged += healthGauge.SetHP;
        }
    }

    private void UpdateHealthUI(float currentHealth)
    {
        Debug.Log($"Player Health: {currentHealth}/{healthPoint.MaxHealth}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            healthPoint.TakeDamage(10f);
        }
    }
}