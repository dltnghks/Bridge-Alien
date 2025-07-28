using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMap : MonoBehaviour
{
    private HurdleSpawner _hurdleSpawner;
    private InfiniteMapManager _infMapManager;
    private SpriteRenderer _ground;

    private Vector2 _groundSize;
    private Rect _groundRect;
    public Rect GroundRect { get { return _groundRect; } }
    
    private float _spawnTimer = .0f;
    private HurdleType _spawnType;

    private bool isActive = false;

    public void UpdateIsActive(bool flag)
    {
        isActive = flag;
        
        _infMapManager?.ChangeActive(isActive);
    }

    public void Initialize()
    {
        if (isActive) return;

        _infMapManager = Utils.FindChild<InfiniteMapManager>(gameObject, "Map", true);
        if (_infMapManager == null)
        {
            Debug.Log("INF MAP이 존재하지 않습니다.");
            return;
        }
        
        _ground = Utils.FindChild<SpriteRenderer>(gameObject, "GroundSprite", true);
        if (_ground == null)
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
        
        _groundSize = _ground.bounds.size;
        _groundRect = new Rect(_ground.bounds.min, _groundSize);
        
        _infMapManager.Initialize();
        _hurdleSpawner.Initialize();
    }
    
    private void Update()
    {
    }
}