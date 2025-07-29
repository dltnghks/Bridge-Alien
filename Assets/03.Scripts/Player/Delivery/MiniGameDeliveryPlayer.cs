using System.Collections;
using UnityEngine;

public class MiniGameDeliveryPlayer : Player
{
    [Header("무적 설정")]
    [SerializeField] private float invincibleTime = 1.5f; // 무적 지속 시간
    [SerializeField] private float blinkInterval = 0.1f;  // 깜빡임 주기

    private bool isInvincible = false;

    public DamageHandler damageHandler;

    public void Start()
    {
        base.Start();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (isInvincible) return;

        if (coll.CompareTag("Enemy"))
        {
            damageHandler.OnDamage();
            UpdateDamageEffect();
            OnDamageEffect();
            
            // TODO: SFX 추가
        }
    }

    private void UpdateDamageEffect()
    {
        if (damageHandler.DamageRate > 0.75f)
        {
            
        }
    }

    private void OnDamageEffect()
    {
        if (!isInvincible)
            StartCoroutine(OnInvincible());
    }

    private IEnumerator OnInvincible()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool isOpaque = true;
        Color originalColor = spriteRenderer.color;

        while (elapsed < invincibleTime)
        {
            float alpha = isOpaque ? 0.3f : 1f;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            isOpaque = !isOpaque;
            
            yield return new WaitForSeconds(blinkInterval);
            
            elapsed += blinkInterval;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
    }
}
