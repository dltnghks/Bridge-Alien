using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameDeliveryPlayer : Player
{
    [Header("무적 설정")]
    [SerializeField] private float bumpInvincibleTime = 1.5f;   // 부딪혔을 때 무적 지속 시간
    [SerializeField] private float skillInvincibleTime = 1.5f;  // 스킬 무적 지속 시간
    [SerializeField] private float blinkInterval = 0.1f;        // 깜빡임 주기
    
    private float _invincibleTime = .0f;

    private bool _isInvincible = false;
    private bool _isPassBump = false;

    private DamageHandler _damageHandler;
    
    private bool _isExiting = false;
    public UnityAction OnExitComplete;

    public void Start()
    {
        base.Start();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    public void SetUp(DamageHandler handler)
    {
        _damageHandler = handler;
    }

    public void StartExitMove()
    {
        _isExiting = true;
        _isInvincible = true;
    }

    private void Update()
    {
        if (_isExiting)
        {
            MoveToRight();

            Vector3 viewPos = Camera.main.WorldToViewportPoint(transform.position);
            if (viewPos.x > 1.1f)
            {
                _isExiting = false;
                OnExitComplete?.Invoke();
            }
        }
    }
    
    public void MoveToRight()
    {
        float speed = 30f;
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (_isInvincible) return;

        if (coll.CompareTag("Enemy"))
        {
            _damageHandler.OnDamage();
            UpdateDamageEffect();
            OnDamageEffect();
            
            // TODO: SFX 추가
        }
    }

    // 데미지 Rate가 100%을 넘었을 때, 특수한 이벤트가 있다면.
    private void UpdateDamageEffect()
    {
        if (_damageHandler.DamageRate > 0.75f)
        {
            
        }
    }

    public void OnRocketEffect(bool isOn)
    {
        Debug.Log("Call Method - RocketEffect   : " + isOn);
        if (isOn)
        {
            moveMultiplier = 2f;
            
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
        if (!_isInvincible)
            StartCoroutine(OnInvincible());
    }

    private IEnumerator OnInvincible()
    {
        _isInvincible = true;

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
        _isInvincible = false;
    }
}
