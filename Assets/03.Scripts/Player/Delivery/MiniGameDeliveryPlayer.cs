using System.Collections;
using UnityEngine;

public class MiniGameDeliveryPlayer : Player
{
    [Header("무적 설정")]
    [SerializeField] private float bumpInvincibleTime = 1.5f; // 부딪혔을 때 무적 지속 시간
    [SerializeField] private float skillInvincibleTime = 1.5f;  // 스킬 무적 지속 시간
    [SerializeField] private float blinkInterval = 0.1f;  // 깜빡임 주기
    
    private float _invincibleTime = .0f;

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

    public void OnRocketEffect(bool isOn)
    {
        Debug.Log("Call Method - RocketEffect   : " + isOn);
        if (isOn)
        {
            moveMultiplier = 5f;
            
            _invincibleTime = skillInvincibleTime;
            
            blinkInterval = 100f;
            StartCoroutine(OnInvincible());
        }
        else
        {
            moveMultiplier = 1.0f;
            StopCoroutine(OnInvincible());
            blinkInterval = 0.1f;
        }
        
        // Rocket 스킬이 발동된다면.
        // 1. 무적 상태가 되어야 한다.
        // 2. 맵과 플레이어 속도가 상승해야 한다.
        
        // 생각할 수 있는 지점. 피격 상태에서 Rocket 스킬을 사용한다면?
        // ㄴ 스킬 사용이 우선시 되어야 하지.
        
    }

    private void OnDamageEffect()
    {
        _invincibleTime = bumpInvincibleTime;
        if (!isInvincible)
            StartCoroutine(OnInvincible());
    }

    private IEnumerator OnInvincible()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool isOpaque = true;
        Color originalColor = spriteRenderer.color;

        while (elapsed < _invincibleTime)
        {
            float alpha = isOpaque ? 1f : 0.3f;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            yield return new WaitForSeconds(blinkInterval);
            
            isOpaque = !isOpaque;
            elapsed += blinkInterval;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
    }
}
