using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    //! 카메라 타입 Enum
    public enum CameraType
    {
        TopDown,
        ThirdPerson
    }

    [Header("Camera Settings")]
    [SerializeField] private CameraType currentCameraType = CameraType.ThirdPerson;         // 기본 카메라 타입 (기본은 3인칭)
    [SerializeField] private Transform target;                                              // 카메라 타겟 (플레이어의 위치)

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeDecay = 0.002f;                                     // 흔들림 감쇠율
    private float currentShakeIntensity = 0f;                                               // 현재 흔들림 강도
    private Vector3 shakeOffset;                                                            // 흔들림 오프셋
    private CameraController currentController;                                             // 현재 카메라 컨트롤러
    private Camera mainCamera;

    public void Init()
    {
        SetupCamera();
    }

    //***************** 초기화 & 업데이트 *****************

    //~ 마지막 업데이트 이후에 흔들림 효과 적용
    private void LateUpdate()
    {
        if (currentShakeIntensity > 0)                                                                   // 흔들림 강도가 0보다 크면                                    
        {
            // 새로운 흔들림 오프셋 계산
            shakeOffset = Random.insideUnitSphere * currentShakeIntensity;               // 흔들림 오프셋 계산      

            // 현재 카메라 컨트롤러가 계산한 위치에 흔들림 효과 추가
            if (mainCamera != null && currentController != null)                           // 메인 카메라와 현재 카메라 컨트롤러가 있으면
            {
                mainCamera.transform.position += shakeOffset;                              // 흔들림 효과 추가
            }

            currentShakeIntensity -= shakeDecay;                                         // 흔들림 강도 감쇠
        }
    }

    //~ 카메라 매니저 초기화    
    public void Initialize(Transform target)
    {
        this.target = target;                                                       // 타겟 설정
        SetupCamera();                                                              // 카메라 설정
    }

    //***************** 카메라 타입 설정 *****************
    //~ 카메라 타입 설정 (Enum CameraType)
    public void SetCameraType(CameraType type)
    {
        currentCameraType = type;                                                    // 카메라 타입 설정    
        SetupCamera();                                                               // 카메라 설정
    }

    //~ 카메라 설정
    private void SetupCamera()
    {
        if (target == null) return;                                                  // 타겟이 없으면 리턴

        mainCamera = Camera.main;                                                // 메인 카메라 찾기    
        if (mainCamera == null)                                                      // 메인 카메라가 없으면
        {
            Logger.LogError("메인 카메라를 찾을 수 없습니다!");                        // 에러 메시지 출력
            return;                                                                  // 리턴
        }

        if (currentController != null) { Destroy(currentController); }               // 현재 카메라 컨트롤러가 있으면 기존 카메라 컨트롤러 제거 

        switch (currentCameraType)                                                              // 카메라 타입에 따라 카메라 컨트롤러 생성
        {
            // 메인 카메라에서 카메라 컨트롤러 컴포넌트를 가져와서 설정
            case CameraType.TopDown:                                                                                     
                currentController = mainCamera.gameObject.GetComponent<TopDownCamera>();          
                break;
            case CameraType.ThirdPerson:                                                        
                currentController = mainCamera.gameObject.GetComponent<ThirdPersonCamera>();       
                break;
        }

        currentController.Initialize(target);                                                   // 카메라 컨트롤러 초기화  
    }

    //***************** Shake Camera *****************
    //~ 카메라 흔들기
    public void ShakeCamera(float intensity, float duration)
    {
        if (Camera.main == null) return;                // 만약 메인 카메라가 없으면 리턴

        currentShakeIntensity = intensity;              // 흔들림 강도 설정
        StartCoroutine(StopShaking(duration));          // 흔들림 중지 코루틴 시작
    }

    //~ 흔들림 중지
    private IEnumerator StopShaking(float duration)
    {
        yield return new WaitForSeconds(duration);      // 지정된 시간 후에 흔들림 중지
        currentShakeIntensity = 0f;                     // 흔들림 강도 초기화
        shakeOffset = Vector3.zero;                     // 흔들림 오프셋 초기화
    }
}
