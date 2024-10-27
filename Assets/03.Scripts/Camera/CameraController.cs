using UnityEngine;

//! 카메라 설정 구조체
[System.Serializable]
public struct CameraSettings
{
    public Vector3 offset;
    public float smoothSpeed;
    public Vector2 rotationSpeed;
    public Vector2 pitchMinMax;
    public float distance;
    
    public static CameraSettings Default => new CameraSettings
    {
        offset = new Vector3(0, 2, -5),
        smoothSpeed = 5f,
        rotationSpeed = new Vector2(3f, 2f),
        pitchMinMax = new Vector2(-30f, 60f),
        distance = 5f
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
