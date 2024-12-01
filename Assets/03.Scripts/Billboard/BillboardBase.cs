using UnityEngine;

//! 빌보드 기본 클래스 
public abstract class BillboardBase : MonoBehaviour 
{
    protected Camera mainCamera;        // 메인 카메라 참조
    
    public enum BillboardAxis
    {
        All,                            // 모든 축으로 회전
        VerticalOnly,                   // Y축 회전만
        HorizontalOnly                  // X축 회전만
    }                   
    
    [SerializeField] public BillboardAxis billboardAxis = BillboardAxis.All;    // protected를 public으로 변경
    [SerializeField] public bool freezeXZAxis = false;                          // protected를 public으로 변경
    
    //~ 초기화
    protected virtual void Awake()
    {
        mainCamera = Camera.main;                                       // 메인 카메라 참조
        
        if (mainCamera == null)                                         // 카메라 예외처리
        {
            Logger.LogError("메인 카메라를 찾을 수 없습니다!");
            enabled = false;
            return;
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
        // 카메라의 rotation을 가져와서 스프라이트에 적용
        Quaternion cameraRotation = mainCamera.transform.rotation;

        switch (billboardAxis)
        {
            case BillboardAxis.All:
                transform.rotation = cameraRotation;
                break;
                
            case BillboardAxis.VerticalOnly:
                transform.rotation = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
                break;
                
            case BillboardAxis.HorizontalOnly:
                transform.rotation = Quaternion.Euler(cameraRotation.eulerAngles.x, 0f, 0f);
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
    protected new ParticleSystem particleSystem;
    
    protected override void Awake()
    {
        base.Awake();
        particleSystem = GetComponent<ParticleSystem>();
        
        var mainModule = particleSystem.main;
        mainModule.scalingMode = ParticleSystemScalingMode.Local;
    }
}