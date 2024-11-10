using UnityEngine;

//! 카메라 설정 구조체
[System.Serializable]
public struct CameraSettings
{
    public Vector3 offset;                       // 카메라 오프셋    
    public float smoothSpeed;                    // 부드러운 카메라 이동 속도
    public Vector2 rotationSpeed;                // 카메라 회전 속도 (좌우, 상하)
    public Vector2 pitchMinMax;                  // 카메라 피치 범위 (상하 각도)
    public float distance;                       // 카메라 거리
    public float defaultPitch;                   // 기본 피치 각도 (피치란 카메라의 상하 각도)
    public float returnSpeed;                    // 돌아가는 속도
    public float minWallDistance;                // 벽과의 최소 거리
    public float sphereCastRadius;               // 구체 캐스트 반경 (벽과의 거리 계산 시 사용)
    public float minZoomDistance;                // 최소 줌 거리 (줌 거리 계산 시 사용)

    public static CameraSettings Default => new CameraSettings
    {
        offset = new Vector3(0, 2, -5),         // 카메라 오프셋
        smoothSpeed = 5f,                       // 부드러운 카메라 이동 속도    
        rotationSpeed = new Vector2(1f, 0.5f),  // 카메라 회전 속도
        pitchMinMax = new Vector2(-30f, 60f),   // 카메라 피치 범위 (상하 각도 제한 (-30도 ~ 60도))
        distance = 30f,                          // 카메라 거리
        defaultPitch = 20f,                      // 추가: 기본 상하 각도 (20도)
        returnSpeed = 10f,            // 추가: 돌아가는 속도
        minWallDistance = 0.5f,       // 추가: 벽과의 최소 거리
        sphereCastRadius = 0.2f,      // 추가: 구체 캐스트 반경
        minZoomDistance = 2f          // 추가: 최소 줌 거리
    };
}

//! 카메라 컨트롤러 기본 클래스
public abstract class CameraController : MonoBehaviour
{
    [SerializeField] protected CameraSettings settings = CameraSettings.Default;
    protected Transform target;
    protected Vector3 currentRotation;
    protected float currentDistance;

    public void Initialize(Transform target)
    {
        this.target = target;
        currentDistance = settings.distance;
        OnInitialized();
    }

    protected virtual void OnInitialized() { }
    protected abstract void UpdateCamera();

    protected virtual void LateUpdate()
    {
        if (target == null) return;
        UpdateCamera();
    }

    public virtual void UpdateSettings(CameraSettings newSettings)
    {
        settings = newSettings;
    }
}
