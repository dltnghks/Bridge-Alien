using UnityEngine;

[RequireComponent(typeof(HousePlayerAnimator))]
public class HousePlayer : Player
{
    private HousePlayerAnimator _animator;
    private Vector2? _targetPosition;
    private Vector3 _initialScale;

    public void Start()
    {
        base.Start();

        rb.useGravity = false;
        _animator = GetComponent<HousePlayerAnimator>();
        _initialScale = transform.localScale;
    }

    protected override void SetAnimator()
    {
        // HousePlayer는 HousePlayerAnimator를 직접 사용, CharacterAnimator 불필요
    }
    
    // 상호작용으로 타겟에게 이동하기 때문에 조건 체크해서 다른 상호작용 불가능하도록 하기
    public void MoveToTarget(Transform target)
    {
        _targetPosition = target.position;
    }
    public void MoveToTarget(Vector2 pos)
    {
        _targetPosition = pos;
    }

    
    private void FixedUpdate()
    {
        if (!_targetPosition.HasValue)
        {
            rb.linearVelocity = Vector2.zero;
            _animator.UpdateSpeed(0f);
            return;
        }

        Vector2 currentPosition = rb.position;
        Vector2 target = _targetPosition.Value;

        if (Vector2.Distance(currentPosition, target) < 0.1f)
        {
            _targetPosition = null; // 목표 도달 시 타겟 초기화
        }
        else
        {
            // 목표를 향해 속도 설정
            Vector2 direction = (target - currentPosition).normalized;
            rb.linearVelocity = direction * MoveSpeed;
            _animator.UpdateSpeed(rb.linearVelocity.magnitude);
            FlipCharacter(rb.linearVelocity.x);
        }
    }

    private void FlipCharacter(float moveDirectionX)
    {
        // 이동 방향이 있을 때만 뒤집기
        if (Mathf.Abs(moveDirectionX) < 0.01f) return;

        if (moveDirectionX > 0)
        {
            IsRight = true;
        }
        else
        {
            IsRight = false;
        }
        
        spriteRenderer.flipX = IsRight;
    }

    public void Rest(bool value)
    {
        _animator.Rest(value);
    }
}

