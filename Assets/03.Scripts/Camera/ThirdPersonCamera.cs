using UnityEngine;

//! 3인칭 카메라 클래스
public class ThirdPersonCamera : CameraController
{
    private Vector3 targetPosition;                     // 타겟 위치
    private float currentPitch;                        // 현재 피치 (상하 회전)
    private float currentYaw;                          // 현재 요 (좌우 회전)
    private float defaultYaw;                         // 추가: 기본 요우 각도
    private bool isBlocked;                            // 레이캐스트 감지 상태
    private float smoothDampVelocity;                  // SmoothDamp용 속도 변수
    private float currentBlockDistance;                // 현재 블록 거리
    private Vector3 previousHitPoint;                  // 이전 히트 포인트
    
    //~ 초기화
    protected override void OnInitialized()
    {
        defaultYaw = target.eulerAngles.y;            
        currentPitch = settings.defaultPitch;                   
        currentYaw = defaultYaw;                       
        currentDistance = settings.distance;           
        currentBlockDistance = currentDistance;
    }
    
    //~ 카메라 업데이트 오버라이드
    protected override void UpdateCamera()
    {
        HandleInput();                                 
        UpdateCameraPosition();                        
    }
    
    //~ 입력 처리
    private void HandleInput()
    {
        /*if (Input.GetMouseButton(1))                   
        {
            currentYaw += Input.GetAxis("Mouse X") * settings.rotationSpeed.x;
            currentPitch -= Input.GetAxis("Mouse Y") * settings.rotationSpeed.y;
            currentPitch = Mathf.Clamp(currentPitch, settings.pitchMinMax.x, settings.pitchMinMax.y);
        }
        else
        {
            currentYaw = Mathf.Lerp(currentYaw, defaultYaw, Time.deltaTime * settings.returnSpeed);
            currentPitch = Mathf.Lerp(currentPitch, settings.defaultPitch, Time.deltaTime * settings.returnSpeed);
        }
        
        if (!isBlocked)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentDistance = Mathf.Clamp(currentDistance - scroll * 5f, 
                settings.minZoomDistance, 
                settings.distance);
            currentBlockDistance = currentDistance;
        }*/
    }
    
    //~ 카메라 위치 계산
    private void UpdateCameraPosition()
    {
        targetPosition = target.position + settings.offset;
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 direction = rotation * Vector3.forward;

        // SphereCast를 사용하여 벽 감지
        RaycastHit hit;
        bool isHit = Physics.SphereCast(
            targetPosition,              // 시작점
            settings.sphereCastRadius,    // 구체 반경
            -direction,                  // 방향
            out hit,                     // 히트 정보
            currentDistance + 0.5f       // 거리
        );

        if (isHit)
        {
            // 벽과의 거리 계산 (sphereCast 반경 고려)
            float distanceToWall = hit.distance + settings.sphereCastRadius;
            
            // 최소 거리 보장
            float targetDistance = Mathf.Max(distanceToWall - settings.minWallDistance, settings.minWallDistance);
            
            // 부드러운 거리 조정
            currentBlockDistance = Mathf.SmoothDamp(
                currentBlockDistance,     // 현재 값
                targetDistance,           // 목표 값
                ref smoothDampVelocity,   // 속도 참조
                0.1f                      // 부드러움 정도
            );
            
            isBlocked = true;
            previousHitPoint = hit.point;
        }
        else
        {
            // 벽에서 벗어날 때 부드럽게 원래 거리로 복귀
            currentBlockDistance = Mathf.SmoothDamp(
                currentBlockDistance, 
                currentDistance,
                ref smoothDampVelocity,
                0.2f
            );
            
            isBlocked = false;
        }

        // 최종 카메라 위치 계산
        Vector3 targetCameraPosition = targetPosition - direction * currentBlockDistance;
        
        // 추가 레이캐스트로 얇은 벽 통과 방지
        RaycastHit wallHit;
        if (Physics.Linecast(targetPosition, targetCameraPosition, out wallHit))
        {
            targetCameraPosition = wallHit.point + (direction * settings.minWallDistance);
        }
        
        // 최종 위치 적용 (부드러운 이동)
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, settings.smoothSpeed * Time.deltaTime);
        
        // 카메라가 항상 타겟을 바라보도록 함
        transform.LookAt(targetPosition);
        
        // 디버그 시각화
        Debug.DrawLine(targetPosition, transform.position, isBlocked ? Color.red : Color.green);
    }

    private void OnDrawGizmos()
    {
        // Scene 뷰에서 SphereCast 범위 시각화
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, settings.sphereCastRadius);
        }
    }
}