using UnityEngine;
using UnityEngine.Events;

public class MiniGameDeliveryPlayerHealthPoint : MonoBehaviour, IHealthPoint
{
    [SerializeField]
    private float maxHealth = 100f;

    private float currentHealth;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public event UnityAction<float, float> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth; // 초기 체력을 최대 체력으로 설정
    }

    public void Heal(float amount)
    {
        if (amount <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (IsDead())
        {
            Debug.Log($"{gameObject.name} is dead!");
            // 죽었을 때의 처리 로직 추가
            Managers.MiniGame.EndGame();
        }
    }

    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}