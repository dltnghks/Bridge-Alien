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

    private static CameraManager instance;                                                  // 카메라 매니저는 싱글톤 패턴으로 구현
    private CameraController currentController;                                             // 현재 카메라 컨트롤러

    [Header("Camera Settings")]
    [SerializeField] private CameraType currentCameraType = CameraType.ThirdPerson;         // 기본 카메라 타입 (기본은 3인칭)
    [SerializeField] private Transform target;                                              // 카메라 타겟 (플레이어의 위치)

    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeDecay = 0.002f;                                     // 흔들림 감쇠율
    private float currentShakeIntensity = 0f;                                               // 현재 흔들림 강도
    private Vector3 shakeOffset;                                                            // 흔들림 오프셋

    [Header("Top Down Camera Settings")]
    [SerializeField] private CameraSettings topDownSettings = CameraSettings.Default;       // 탑 다운 카메라 설정은 CameraController의 Setting 에서 가져옴

    [Header("Third Person Camera Settings")]
    [SerializeField] private CameraSettings thirdPersonSettings = CameraSettings.Default;   // 세 번째 인치 카메라 설정은 CameraController의 Setting 에서 가져옴

    private static CameraManager instance;                                                  // 카메라 매니저는 싱글톤 패턴으로 구현
    private CameraController currentController;                                             // 현재 카메라 컨트롤러

    public void Init(CameraType cameraType, CameraSettings cameraSettings)
    {
        currentCameraType = cameraType;
        if (cameraType == CameraType.ThirdPerson)
        {
            thirdPersonSettings = cameraSettings;
        }
        else if (cameraType == CameraType.TopDown)
        {
            topDownSettings = cameraSettings;
        }
        
        SetupCamera();
    }

    //***************** 초기화 & 업데이트 *****************
    //~ Awake시, 카메라 매니저 인스턴스 생성
    private void Awake()
    {
        if (instance == null)                                                       // 인스턴스가 없어?
        {
            instance = this;                                                        // 인스턴스 설정
            DontDestroyOnLoad(gameObject);                                          // 씬 로드 시 카메라 매니저 제거 방지
        }
        else
        {
            Destroy(gameObject);                                                    // 씬에 이미 카메라 매니저가 존재하므로 중복 생성 방지를 위해 현재 오브젝트 제거
        }
    }

    //~ 마지막 업데이트 이후에 흔들림 효과 적용
    private void LateUpdate()
    {
        if (currentShakeIntensity > 0)                                                                   // 흔들림 강도가 0보다 크면                                    
        {
            // 새로운 흔들림 오프셋 계산
            shakeOffset = Random.insideUnitSphere * currentShakeIntensity;               // 흔들림 오프셋 계산      

            // 현재 카메라 컨트롤러가 계산한 위치에 흔들림 효과 추가
            if (Camera.main != null && currentController != null)                           // 메인 카메라와 현재 카메라 컨트롤러가 있으면
            {
                Camera.main.transform.position += shakeOffset;                              // 흔들림 효과 추가
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

        var mainCamera = Camera.main;                                                // 메인 카메라 찾기    
        if (mainCamera == null)                                                      // 메인 카메라가 없으면
        {
            Logger.LogError("메인 카메라를 찾을 수 없습니다!");                        // 에러 메시지 출력
            return;                                                                  // 리턴
        }

        if (currentController != null) { Destroy(currentController); }               // 현재 카메라 컨트롤러가 있으면 기존 카메라 컨트롤러 제거 

        switch (currentCameraType)                                                              // 카메라 타입에 따라 카메라 컨트롤러 생성
        {
            case CameraType.TopDown:                                                            // 탑 다운 카메라 타입                         
                currentController = mainCamera.gameObject.AddComponent<TopDownCamera>();        // 탑 다운 카메라 컨트롤러 생성
                currentController.UpdateSettings(topDownSettings);                              // 탑 다운 카메라 설정 적용   
                break;
            case CameraType.ThirdPerson:                                                        // 세 번째 인치 카메라 타입
                currentController = mainCamera.gameObject.AddComponent<ThirdPersonCamera>();    // 세 번째 인치 카메라 컨트롤러 생성
                currentController.UpdateSettings(thirdPersonSettings);                          // 세 번째 인치 카메라 설정 적용   
                break;
        }

        currentController.Initialize(target);                                                   // 카메라 컨트롤러 초기화  
    }

    //~ 현재 카메라 설정 업데이트
    public void UpdateCurrentCameraSettings()
    {
        if (currentController == null) return;                                                  // 현재 카메라 컨트롤러가 없으면 리턴  

        switch (currentCameraType)                                                              // 카메라 타입에 따라 카메라 설정 업데이트
        {
            case CameraType.TopDown:                                                            // 탑 다운 카메라 타입                         
                currentController.UpdateSettings(topDownSettings);                              // 탑 다운 카메라 설정 적용   
                break;
            case CameraType.ThirdPerson:                                                        // 세 번째 인치 카메라 타입
                currentController.UpdateSettings(thirdPersonSettings);                          // 세 번째 인치 카메라 설정 적용   
                break;
        }
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
