using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    protected enum Direction { Forward, Back, Left, Right }
    
    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 8f;      // 이동 속도
    [SerializeField] protected float moveMultiplier = 1f;   // 일반 이동 속도
    [SerializeField] protected float moveAdditionalMultiplier = 1f;
    [SerializeField] protected float buttonMoveMultiplier = 1f;
    [SerializeField] protected float rayDistance = 1f;    // 레이캐스트 거리
    [SerializeField] protected bool enableFlip = true;    // 플레이어 플립 활성화 여부
    [SerializeField] protected bool isHit = false;        // 플레이어가 맞았는지 여부

    protected GameObject spriteObject;                    // 스프라이트 오브젝트
    protected SpriteBillboard billboard;                  // 스프라이트 빌보드
    protected SpriteRenderer spriteRenderer;              // 스프라이트 렌더러
    protected Rigidbody rb;                               // 리지드바디
    protected bool[] canMove = { true, true, true, true };
    protected Vector3[] directions = {
        Vector3.forward,  // 앞쪽
        Vector3.back,     // 뒤쪽
        Vector3.left,     // 왼쪽
        Vector3.right     // 오른쪽
    };
    
    protected CharacterAnimator characterAnimator;        // 캐릭터 애니메이터
    
    protected GameObject playerBody;
    
    public bool IsRight { get; protected set; }
    public bool IsHit {
        get
        {
            return isHit;
        }
        protected set {}
    }

    public Transform CharacterTransform => playerBody.transform;

    public float MoveSpeed => moveSpeed * moveMultiplier * moveAdditionalMultiplier * buttonMoveMultiplier;

    public void Start()
    {
        // 리지드바디 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null){ rb = gameObject.AddComponent<Rigidbody>(); }

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;            

        // 자식 스프라이트 오브젝트 생성
        playerBody = new GameObject("PlayerBody");
        playerBody.transform.SetParent(transform);
        // 박스 들 때, Sprite가 겹쳐서 앞으로 살짝 뺌
        playerBody.transform.localPosition = -Vector3.forward/10f;

        IsRight = true;
        
        // 스프라이트 오브젝트 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 캐릭터 애니메이터 설정
        SetAnimator();
    }

    protected virtual void SetAnimator()
    {
        // 캐릭터 애니메이터 설정
        characterAnimator = GetComponent<CharacterAnimator>();
        if (characterAnimator == null)
        {
            characterAnimator = gameObject.AddComponent<CharacterAnimator>();
        }
    }

    protected void Update()
    {
        CheckCollisions();
    }
    
    protected void CheckCollisions()
    {
        for (int i = 0; i < directions.Length; i++)
        {
            RaycastHit hit;
            Vector3 direction = directions[i];
            if (Physics.Raycast(transform.position, direction, out hit, rayDistance))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    canMove[i] = false;    
                }
                else
                {
                    canMove[i] = true;
                }
            }
            else
            {
                canMove[i] = true;
            }
        }

        /*for (int i = 0; i < canMove.Length; i++)
        {
            Logger.Log($"{i} : {canMove[i]}");
        }*/
    }
    
    public void PlayerMovement(Vector2 joystickInput)
    {
        float horizontal = joystickInput.x;
        float vertical = joystickInput.y;
        Vector3 movement = Vector3.zero;  // 이동 벡터 초기화

        // x축(좌우) 이동과 스프라이트 방향 전환
        if (horizontal > 0 && canMove[(int)Direction.Right])
        {
            movement += transform.right * horizontal;
            playerBody.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            IsRight = true;
        }
        else if (horizontal < 0 && canMove[(int)Direction.Left])
        {
            movement += transform.right * horizontal;
            playerBody.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            IsRight = false;
        }
        
        // 회전이 가능한 경우에만 진행
        if(enableFlip)
        {
            spriteRenderer.flipX = IsRight;
        }

        // z축(전후) 이동 - 벽 체크 포함
        if (Mathf.Abs(vertical) > 0.01f)
        {
            if (vertical > 0 && canMove[(int)Direction.Forward])
            {  
                movement += transform.forward * vertical;
            }
            else if (vertical < 0 && canMove[(int)Direction.Back])
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
        Vector3 horizontalVelocity = movement * moveSpeed;
        horizontalVelocity.y = rb.velocity.y;  // 기존 수직 속도 유지
        rb.velocity = horizontalVelocity;
        rb.angularVelocity = Vector3.zero;
            
        // 캐릭터 애니메이터 업데이트
        characterAnimator.UpdateMovement(movement.magnitude * moveSpeed);
    }

    public void PlayWinPose()
    {
        characterAnimator.PlayWinPose();
    }

    public void PlayLosePose()
    {
        characterAnimator.PlayLosePose();
    }

    public void EventFootStepSound()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.FootStepPlayerCharacter.ToString());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * rayDistance);
        Gizmos.DrawRay(transform.position, -transform.forward * rayDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.right * rayDistance);
        Gizmos.DrawRay(transform.position, -transform.right * rayDistance);
    }

    public void SpeedUp(float speedBoost)
    {
        moveSpeed += speedBoost;
    }
}