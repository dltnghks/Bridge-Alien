using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMap : MonoBehaviour
{
    private HurdleSpawner _hurdleSpawner;
    private InfiniteMapManager _infMapManager;

    [Header("무한 스크롤 속도")]
    [SerializeField] private float cloudSpeed = .0f;
    [SerializeField] private float buildSpeed = .0f;
    [SerializeField] private float fenceSpeed = .0f;
    [SerializeField] private float groundSpeed = .0f;

    public float GroundSpeed { get { return groundSpeed; } }

    [SerializeField, Space(10)] private float speedMultiplier = 1.0f;

    public Rect GroundRect { get; private set; }

    private bool _isActive = false;

    public void Initialize()
    {
        if (_isActive) return;

        _infMapManager = Utils.FindChild<InfiniteMapManager>(gameObject, "Map", true);
        if (_infMapManager == null)
        {
            Debug.Log("INF MAP이 존재하지 않습니다.");
            return;
        }
        
        var ground = Utils.FindChild<SpriteRenderer>(gameObject, "GroundSprite", true);
        if (ground == null)
        {
            Debug.Log("Ground가 없습니다.");
            return;
        }
        
        _hurdleSpawner = Utils.FindChild<HurdleSpawner>(gameObject, "Map", true);
        if (_hurdleSpawner == null)
        {
            Debug.Log("Hurdle 스폰 불가!");
            return;
        }
        
        var groundSize = ground.bounds.size;
        GroundRect = new Rect(ground.bounds.min, groundSize);
        
        _infMapManager.Initialize();
        UpdateSpeedMultiplier();
        
        // 허들도 동일한 수치를 위해서 속도 전달
        _hurdleSpawner.Initialize(groundSpeed);
    }
    
    public void UpdateIsActive(bool flag)
    {
        _isActive = flag;
        
        _infMapManager?.ChangeActive(_isActive);
    }
    
    // 가속을 위한 메서드
    public void UpdateSpeedMultiplier(float multiplier = 1f)
    {
        speedMultiplier = multiplier;
        _infMapManager.SetSpeeds(
            cloudSpeed * speedMultiplier,
            buildSpeed * speedMultiplier,
            fenceSpeed * speedMultiplier,
            groundSpeed * speedMultiplier
        );
        _hurdleSpawner.ChangeGroundSpeed(groundSpeed * speedMultiplier);
        groundSpeed *= speedMultiplier;
    }
}