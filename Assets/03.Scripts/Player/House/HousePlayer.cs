using UnityEngine;

[RequireComponent(typeof(HousePlayerAnimator))]
public class HousePlayer : Player
{
    [SerializeField] private BoxCollider2D movementArea;

    private HousePlayerAnimator _animator;
    private Vector2? _targetPosition;
    private Vector3 _initialScale;

    private bool _isInteract;

    public void Start()
    {
        base.Start();
        
        _isInteract = false;
        rb.useGravity = false;
        _animator = GetComponent<HousePlayerAnimator>();
        _initialScale = transform.localScale;
    }

    private void Update()
    {
        if(_isInteract == true) return;

        // 화면에 터치가 하나 이상 감지되면
        if (Input.touchCount > 0)
        {
            // 첫 번째 터치 정보를 가져옴
            Touch touch = Input.GetTouch(0);

            // 터치가 시작되는 순간을 감지
            if (touch.phase == TouchPhase.Began)
            {
                HandleInput(touch.position);
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }
#endif        
    }

    // 상호작용으로 타겟에게 이동하기 때문에 조건 체크해서 다른 상호작용 불가능하도록 하기
    public void MoveToTarget(Transform target)
    {
        if(_isInteract == true) return;
        
        HandleInput(target.position);
    }

    public void StartInteract()
    {
        _isInteract = true;
    }

    public void EndInteract()
    {
        _isInteract = false;
    }

    /**
    * 화면 좌표(screenPosition)를 받아 Raycast를 수행하는 공통 함수
    */
    private void HandleInput(Vector2 screenPosition)
    {
        // 1. 화면 좌표를 월드 좌표로 변환
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        
        // 2. Raycast 발사
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        // 이동을 위한 타겟 위치 설정
        if (movementArea.bounds.Contains(worldPosition))
        {
            _targetPosition = worldPosition;
        }
    }

    private void FixedUpdate()
    {
        if (!_targetPosition.HasValue)
        {
            rb.velocity = Vector2.zero;
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
            rb.velocity = direction * MoveSpeed;
            _animator.UpdateSpeed(rb.velocity.magnitude);
            FlipCharacter(rb.velocity.x);
        }
    }

    private void LateUpdate()
    {
        if (movementArea == null) return;

        // 물리 위치를 영역 내로 제한
        Vector2 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, movementArea.bounds.min.x, movementArea.bounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, movementArea.bounds.min.y, movementArea.bounds.max.y);
        rb.position = clampedPosition;
    }

    private void FlipCharacter(float moveDirectionX)
    {
        // 이동 방향이 있을 때만 뒤집기
        if (Mathf.Abs(moveDirectionX) < 0.01f) return;

        if (moveDirectionX > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(_initialScale.x), _initialScale.y, _initialScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(_initialScale.x), _initialScale.y, _initialScale.z);
        }
    }

    public void Rest(bool value)
    {
        _animator.Rest(value);
    }
}

