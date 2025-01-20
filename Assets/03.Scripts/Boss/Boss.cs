using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Boss : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float floatHeight = 1.0f;     // 둥둥 떠다니는 높이
    [SerializeField] private float floatSpeed = 1.0f;      // 둥둥 떠다니는 속도
    
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 3.0f;     // 플레이어 추적 속도
    
    private Rigidbody rb;                                  // 리지드바디
    private Vector3 startPosition;                         // 시작 위치
    private float floatTimer = 0f;                         // 타이머
    private Transform playerTransform;                     // 플레이어의 Transform
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;                            // 중력 영향 받지 않도록 설정
        rb.isKinematic = true;                            // 물리 영향 받지 않도록 설정
        
        startPosition = transform.position;                // 시작 위치 저장
        
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            Logger.Log("플레이어를 찾았습니다!");
        }
        else
        {
            Logger.LogError("플레이어를 찾을 수 없습니다!");
        }
    }
    
    private void Update()
    {
        // 상하 움직임 계산
        floatTimer += Time.deltaTime;
        float newY = startPosition.y + Mathf.Sin(floatTimer * floatSpeed) * floatHeight;
        
        // 플레이어 Z축 추적
        float targetZ = playerTransform != null ? playerTransform.position.z : transform.position.z;
        float newZ = Mathf.Lerp(transform.position.z, targetZ, Time.deltaTime * followSpeed);
        
        // 새로운 위치 적용 (X축은 고정, Y축은 둥둥 떠다님, Z축은 플레이어 추적)
        Vector3 newPosition = new Vector3(
            transform.position.x,
            newY,
            newZ
        );
        transform.position = newPosition;
    }
}
