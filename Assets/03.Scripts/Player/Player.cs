using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;      // 이동 속도
    [SerializeField] private float rayDistance = 1f;    // 레이캐스트 거리

    private SpriteBillboard billboard;                  // 스프라이트 빌보드
    private SpriteRenderer spriteRenderer;              // 스프라이트 렌더러
    private bool canMoveForward = true;                 // 전방 이동 가능 여부
    private bool canMoveBackward = true;                // 후방 이동 가능 여부

    //~ Start함수에서는 필요한 컴포넌트들 가져오기/추가하기
    void Start()
    {
        // 필요한 컴포넌트들 가져오기/추가하기
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) { spriteRenderer = gameObject.AddComponent<SpriteRenderer>(); }

        // SpriteBillboard 컴포넌트 추가
        billboard = gameObject.AddComponent<SpriteBillboard>();
        
        // 카메라 설정
        Camera.main.gameObject.AddComponent<ThirdPersonCamera>().Initialize(transform);
    }

    //~ Update 함수에서는 충돌 체크와 이동 처리를 실행합니다.
    void Update()
    {
        CheckCollisions();
        PlayerMovement();
    }

    //~ 충돌 체크
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

    //~ 이동 처리
    void PlayerMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = Vector3.zero;  // 이동 벡터 초기화

        // x축(좌우) 이동
        movement += transform.right * horizontal;

        // z축(전후) 이동 - 벽 체크 포함
        if (Mathf.Abs(vertical) > 0.01f)  // vertical 입력이 있을 때만 처리
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
        
        transform.position += movement * Time.deltaTime * moveSpeed;
    }

    //~ 디버그용 레이캐스트 시각화
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
        Gizmos.DrawRay(transform.position, -transform.forward * rayDistance);
    }
}