using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;      // 이동 속도
    [SerializeField] private float rayDistance = 1f;    // 레이캐스트 거리

    private GameObject spriteObject;                    // 스프라이트 오브젝트
    private SpriteBillboard billboard;                  // 스프라이트 빌보드
    private SpriteRenderer spriteRenderer;              // 스프라이트 렌더러
    private Rigidbody rb;                              // 리지드바디
    private bool canMoveForward = true;                 // 전방 이동 가능 여부
    private bool canMoveBackward = true;                // 후방 이동 가능 여부

    void Start()
    {
        // 리지드바디 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null) { rb = gameObject.AddComponent<Rigidbody>(); }
        
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;

        // 스프라이트 오브젝트 설정
        SetupSpriteObject();
        
        // 카메라 설정
        Camera.main.gameObject.AddComponent<ThirdPersonCamera>().Initialize(transform);
    }

    private void SetupSpriteObject()
    {
        // 부모의 스프라이트 렌더러에서 스프라이트 가져오기
        SpriteRenderer parentRenderer = GetComponent<SpriteRenderer>();
        Sprite originalSprite = parentRenderer ? parentRenderer.sprite : null;
        
        // 자식 스프라이트 오브젝트 생성
        spriteObject = new GameObject("PlayerSprite");
        spriteObject.transform.SetParent(transform);
        spriteObject.transform.localPosition = Vector3.zero;
        
        // 스프라이트 렌더러 설정
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        
        // 원본 스프라이트와 기타 속성들 복사
        if (parentRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
            spriteRenderer.sortingOrder = parentRenderer.sortingOrder;
            spriteRenderer.color = parentRenderer.color;
            spriteRenderer.flipX = parentRenderer.flipX;
            spriteRenderer.flipY = parentRenderer.flipY;
            spriteRenderer.sortingLayerID = parentRenderer.sortingLayerID;
        }
        
        // 기존 부모의 스프라이트 렌더러 제거
        if (parentRenderer != null)
        {
            Destroy(parentRenderer);
        }
        
        // 빌보드는 스프라이트 오브젝트에 추가
        billboard = spriteObject.AddComponent<SpriteBillboard>();
        billboard.billboardAxis = BillboardBase.BillboardAxis.All;
        // freezeXZAxis는 기본값(false) 사용
    }

    void Update()
    {
        CheckCollisions();
    }

    void FixedUpdate()
    {
        PlayerMovement();
    }

    void CheckCollisions()
    {
        // 전방 레이캐스트
        RaycastHit hitForward;
        if (Physics.Raycast(transform.position, transform.forward, out hitForward, rayDistance))
        {
            if (hitForward.collider.CompareTag("Wall"))
            {
                canMoveForward = false;
            }
        }
        else
        {
            canMoveForward = true;
        }

        // 후방 레이캐스트
        RaycastHit hitBackward;
        if (Physics.Raycast(transform.position, -transform.forward, out hitBackward, rayDistance))
        {
            if (hitBackward.collider.CompareTag("Wall"))
            {
                canMoveBackward = false;
            }
        }
        else
        {
            canMoveBackward = true;
        }
    }

    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = Vector3.zero;  // 이동 벡터 초기화

        // x축(좌우) 이동과 스프라이트 방향 전환
        movement += transform.right * horizontal;
        
        // 스프라이트 방향 전환
        if (Mathf.Abs(horizontal) > 0.01f)  // 좌우 이동이 있을 때만
        {
            Vector3 scale = spriteObject.transform.localScale;
            if (horizontal > 0 && scale.x < 0 || horizontal < 0 && scale.x > 0)
            {
                scale.x *= -1;
                spriteObject.transform.localScale = scale;
            }
        }

        // z축(전후) 이동 - 벽 체크 포함
        if (Mathf.Abs(vertical) > 0.01f)
        {
            if (vertical > 0 && canMoveForward)
            {
                movement += transform.forward * vertical;
            }
            else if (vertical < 0 && canMoveBackward)
            {
                movement += transform.forward * vertical;
            }
        }

        // 이동 속도 정규화 및 적용
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Rigidbody를 통한 이동
        rb.velocity = movement * moveSpeed;
        rb.angularVelocity = Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
        Gizmos.DrawRay(transform.position, -transform.forward * rayDistance);
    }
}