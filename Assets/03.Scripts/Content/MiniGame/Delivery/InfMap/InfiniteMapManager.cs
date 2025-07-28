using UnityEngine;

public class InfiniteMapManager : MonoBehaviour
{
    [Header("Cloud")]
    public Transform[] cloudObjects;
    public float cloudSpeed;

    [Header("Background")]
    public Transform[] buildObjects;
    public float buildSpeed;
    public Transform[] fenceObjects;
    public float fenceSpeed;

    [Header("Ground")]
    public Material groundMaterial;
    public float groundSpeed;

    private InfiniteObjectScroller cloud;
    private InfiniteObjectScroller build;
    private InfiniteObjectScroller fence;
    private GroundInfinite ground;

    [Header("All Settings")]
    private bool _isActive;
    
    public float speedRatio = 1f;

    [Space(10)]
    public float cloudOffset = .0f;

    public void Initialize()
    {
        cloud = new InfiniteObjectScroller();
        cloud.Initialize(cloudObjects, cloudSpeed ,cloudOffset);

        build = new InfiniteObjectScroller();
        build.Initialize(buildObjects, buildSpeed);

        fence = new InfiniteObjectScroller();
        fence.Initialize(fenceObjects, fenceSpeed);

        ground = new GroundInfinite();
        ground.Initialize(groundMaterial, groundSpeed);
    }

    private void Update()
    {
        if (_isActive == false) return;
        
        cloud?.Scroll(speedRatio);
        build?.Scroll(speedRatio);
        fence?.Scroll(speedRatio);
        ground?.Scroll(speedRatio);
    }

    public void ChangeActive(bool flag)
    {
        _isActive = flag;
    }
    
    public void SetSpeeds(float cloudSpeed, float buildSpeed, float fenceSpeed, float groundSpeed)
    {
        cloud?.SetSpeed(cloudSpeed);
        build?.SetSpeed(buildSpeed);
        fence?.SetSpeed(fenceSpeed);
        ground?.SetSpeed(groundSpeed);
    }
}
