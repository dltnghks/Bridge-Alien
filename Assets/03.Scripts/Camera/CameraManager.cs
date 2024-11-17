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
    [SerializeField] private CameraType currentCameraType = CameraType.ThirdPerson; // 기본 카메라 타입
    [SerializeField] private Transform target;                                      // 카메라 타겟

    [Header("Top Down Camera Settings")]
    [SerializeField] private CameraSettings topDownSettings = CameraSettings.Default;

    [Header("Third Person Camera Settings")]
    [SerializeField] private CameraSettings thirdPersonSettings = CameraSettings.Default;

    private static CameraManager instance;                                          // 싱글톤 인스턴스
    private CameraController currentController;                                     // 현재 카메라 컨트롤러

    //! 싱글톤 인스턴스 접근자
    public static CameraManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CameraManager>();                       // 씬에서 카메라 매니저 찾기
                if (instance == null)
                {
                    GameObject go = new GameObject("CameraManager");                // 새로운 게임 오브젝트 생성
                    instance = go.AddComponent<CameraManager>();                    // 카메라 매니저 컴포넌트 추가
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(Transform target)
    {
        this.target = target;
        SetupCamera();
    }

    public void SetCameraType(CameraType type)
    {
        currentCameraType = type;
        SetupCamera();
    }

    private void SetupCamera()
    {
        if (target == null) return;

        var mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("메인 카메라를 찾을 수 없습니다!");
            return;
        }

        // 기존 카메라 컨트롤러 제거
        if (currentController != null)
        {
            Destroy(currentController);
        }

        // 새로운 카메라 컨트롤러 추가
        switch (currentCameraType)
        {
            case CameraType.TopDown:
                currentController = mainCamera.gameObject.AddComponent<TopDownCamera>();
                currentController.UpdateSettings(topDownSettings);
                break;
            case CameraType.ThirdPerson:
                currentController = mainCamera.gameObject.AddComponent<ThirdPersonCamera>();
                currentController.UpdateSettings(thirdPersonSettings);
                break;
        }

        currentController.Initialize(target);
    }
}
