using UnityEngine.Events;

public interface IHealthPoint
{
    // 체력 최대치
    float MaxHealth { get; }

    // 현재 체력
    float CurrentHealth { get; }

    // 체력이 변경될 때 발생하는 이벤트 (현재 체력, 최대 체력)
    event UnityAction<float, float> OnHealthChanged;

    // 체력을 회복
    void Heal(float amount);

    // 체력을 감소 (데미지)
    void TakeDamage(float damage);

    // 현재 체력을 설정 (관리용)
    void SetHealth(float health);

    // 체력이 0 이하인지 확인
    bool IsDead();
}