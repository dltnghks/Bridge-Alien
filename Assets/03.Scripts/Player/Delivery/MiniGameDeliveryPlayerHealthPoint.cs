using UnityEngine;
using UnityEngine.Events;

public class MiniGameDeliveryPlayerHealthPoint : MonoBehaviour, IHealthPoint
{
    [SerializeField, Range(0f, 100f)]
    private float maxHealth = 100f;
    private float _currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => _currentHealth;

    public event UnityAction<float, float> OnHealthChanged;

    private void Awake()
    {
        _currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;

        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);

        if (IsDead())
        {
            Debug.Log($"{gameObject.name} is dead!");
            // 죽었을 때의 처리 로직 추가
            Managers.MiniGame.EndGame();
        }
    }

    public void SetHealth(float health)
    {
        _currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(_currentHealth, maxHealth);
    }

    public bool IsDead()
    {
        return _currentHealth <= 0;
    }
}