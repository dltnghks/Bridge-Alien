using UnityEngine;

//! 탑다운 뷰 카메라 클래스
public class TopDownCamera : CameraController
{
    private float currentZoom;                   // 현재 줌 레벨
    private Vector3 targetPosition;              // 타겟 위치
    
    //~ 초기화
    protected override void OnInitialized()
    {
        currentRotation.y = 0f;
        currentZoom = settings.distance;
        transform.rotation = Quaternion.Euler(90f, currentRotation.y, 0f); // 시작시 90도로 내려다보기
    }
    
    //~ 카메라 업데이트
    protected override void UpdateCamera()
    {
        HandleInput();
        UpdateCameraPosition();
    }
    
    //~ 입력 처리
    private void HandleInput()
    {
        // 우클릭 드래그로 회전
        if (Input.GetMouseButton(1))
        {
            currentRotation.y += Input.GetAxis("Mouse X") * settings.rotationSpeed.x;
        }
        
        // 마우스 휠로 줌 인/아웃
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom = Mathf.Clamp(
            currentZoom - scroll * 5f,
            settings.minZoomDistance,
            settings.distance
        );
    }
    
    //~ 카메라 위치 업데이트
    private void UpdateCameraPosition()
    {
        // 타겟 위치 계산
        targetPosition = target.position + settings.offset;
        
        // 회전 적용
        Quaternion targetRotation = Quaternion.Euler(90f, currentRotation.y, 0f);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * settings.smoothSpeed
        );
        
        // 카메라 위치 계산
        Vector3 targetCameraPosition = targetPosition + (Vector3.up * currentZoom);
        
        // 장애물 체크
        RaycastHit hit;
        if (Physics.Linecast(targetPosition, targetCameraPosition, out hit))
        {
            targetCameraPosition = hit.point + (Vector3.up * settings.minWallDistance);
        }
        
        // 부드러운 이동 적용
        transform.position = Vector3.Lerp(
            transform.position,
            targetCameraPosition,
            Time.deltaTime * settings.smoothSpeed
        );
    }
    
    //~ 디버그 시각화
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(targetPosition, transform.position);
        }
    }
}
