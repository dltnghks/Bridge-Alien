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

    private MGDCharacterAnimator _characterAnimator;

    private bool _isExiting = false;
    public UnityAction OnExitComplete;

    public void Start()
    {
        base.Start();

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    protected override void SetAnimator()
    {
        _characterAnimator = GetComponentInChildren<MGDCharacterAnimator>();
        if (characterAnimator == null)
        {
            _characterAnimator = gameObject.AddComponent<MGDCharacterAnimator>();
        }
        characterAnimator = _characterAnimator;
    }

    public void SetUp(DamageHandler handler)
    {
        _damageHandler = handler;
    }

    public void OnCrash()
    {
        _characterAnimator.SetCrash(true);
    }

    public void OnMove(float speed)
    {
        characterAnimator.UpdateMovement(speed);
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
            OnDamageEffect();

            _characterAnimator.SetHit(true);
            isHit = true;

            Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Player_BeAttacked.ToString());
        }
    }

    public void OnRocketEffect(bool isOn)
    {
        if (isOn)
        {
            moveMultiplier = 2f;
            _invincibleTime = 3f;
            blinkInterval = 3f;

            StartCoroutine(OnInvincible());
        }
        else
        {
            moveMultiplier = 1.0f;
            blinkInterval = 0.1f;
        }
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
        isHit = false;
    }

    public void PlayIdleFlying()
    {
        Managers.Sound.PlaySFX(SoundType.MiniGameDeliverySFX, MiniGameDeliverySoundSFX.Idle_Flying.ToString());
    }
}
