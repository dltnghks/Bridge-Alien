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
    
    public static CameraSettings Default => new CameraSettings
    {
        offset = new Vector3(0, 2, -5),         // 카메라 오프셋
        smoothSpeed = 5f,                       // 부드러운 카메라 이동 속도    
        rotationSpeed = new Vector2(3f, 2f),    // 카메라 회전 속도
        pitchMinMax = new Vector2(-30f, 60f),   // 카메라 피치 범위
        distance = 5f                           // 카메라 거리
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
