using UnityEngine;

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

    [Header("Top Down Camera Settings")]
    [SerializeField] private CameraSettings topDownSettings = CameraSettings.Default;       // 탑 다운 카메라 설정은 CameraController의 Setting 에서 가져옴

    [Header("Third Person Camera Settings")]
    [SerializeField] private CameraSettings thirdPersonSettings = CameraSettings.Default;   // 세 번째 인치 카메라 설정은 CameraController의 Setting 에서 가져옴

    private static CameraManager instance;                                                  // 카메라 매니저는 싱글톤 패턴으로 구현
    private CameraController currentController;                                             // 현재 카메라 컨트롤러

    public void Init()
    {
        
    }
    
    //! 카메라 매니저 초기화    
    public void Initialize(Transform target)
    {
        this.target = target;                                                       // 타겟 설정
        SetupCamera();                                                              // 카메라 설정
    }

    //! 카메라 타입 설정 (Enum CameraType)
    public void SetCameraType(CameraType type)
    {
        currentCameraType = type;                                                    // 카메라 타입 설정    
        SetupCamera();                                                               // 카메라 설정
    }

    //! 카메라 설정
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

    //! 현재 카메라 설정 업데이트
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
}
