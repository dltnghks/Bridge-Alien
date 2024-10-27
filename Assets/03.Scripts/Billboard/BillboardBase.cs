using UnityEngine;

//! 빌보드 기본 클래스
public abstract class BillboardBase : MonoBehaviour
{
    protected Camera mainCamera;        // 메인 카메라 참조
    protected Rigidbody rb;             // 리지드바디 참조 (리지드 바디 회전 제한을 위해 필요)
    
    public enum BillboardAxis
    {
        All,                            // 모든 축으로 회전
        VerticalOnly,                   // Y축 회전만
        HorizontalOnly                  // X축 회전만
    }                   
    
    [SerializeField] protected BillboardAxis billboardAxis = BillboardAxis.All;    // 빌보드 축 설정
    [SerializeField] protected bool freezeXZAxis = false;                          // XZ 축 회전 제한 여부
    
    //~ 초기화
    protected virtual void Awake()
    {
        mainCamera = Camera.main;                                       // 메인 카메라 참조
        rb = GetComponent<Rigidbody>();                                 // 리지드바디 참조
        
        if (mainCamera == null)                                         // 카메라 예외처리
        {
            Debug.LogError("메인 카메라를 찾을 수 없습니다!");
            enabled = false;
            return;
        }

        if (rb != null)                                                 // 리지드바디 예외처리
        {
            // Rigidbody의 회전을 제한
            rb.freezeRotation = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    //~ 업데이트 (카메라 관련 처리라 LateUpdate를 사용함)
    protected virtual void LateUpdate()
    {
        if (mainCamera == null) return;                                // 카메라 예외처리   
        ApplyBillboard();                                              // 빌보드 적용
    }   
    
    //~ 빌보드 적용
    protected virtual void ApplyBillboard()
    {
        Vector3 targetPosition = transform.position + mainCamera.transform.rotation * Vector3.forward;
        Vector3 targetUp = mainCamera.transform.rotation * Vector3.up;

        switch (billboardAxis)
        {
            case BillboardAxis.All:
                transform.LookAt(targetPosition, targetUp);
                break;
                
            case BillboardAxis.VerticalOnly:
                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);
                break;
                
            case BillboardAxis.HorizontalOnly:
                targetPosition = new Vector3(transform.position.x, targetPosition.y, transform.position.z);
                transform.LookAt(targetPosition, Vector3.up);
                break;
        }

        if (freezeXZAxis)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;
        }
    }
}

//! 스프라이트 빌보드
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteBillboard : BillboardBase
{
    protected SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}

//! 캔버스 빌보드
[RequireComponent(typeof(Canvas))]
public class CanvasBillboard : BillboardBase
{
    protected Canvas canvas;
    
    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
    }
}

//! 파티클 빌보드
[RequireComponent(typeof(ParticleSystem))]
public class ParticleBillboard : BillboardBase
{
    protected ParticleSystem particleSystem;
    
    protected override void Awake()
    {
        base.Awake();
        particleSystem = GetComponent<ParticleSystem>();
        
        var mainModule = particleSystem.main;
        mainModule.scalingMode = ParticleSystemScalingMode.Local;
    }
}