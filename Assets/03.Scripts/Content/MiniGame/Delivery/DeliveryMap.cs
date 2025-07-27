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

    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    
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
        
        _groundSize = _ground.bounds.size;
        _groundRect = new Rect(_ground.bounds.min, _groundSize);
        
        _infMapManager.Initialize();
    }
    
    private void Update()
    {
    }
    
    // private void SpawnOnMap()
    // {
    //     SetCriteriaTime();
    //
    //     // 패턴 정보를 넘겨준다.
    //     int patternNum = (int)_spawnType;
    //     
    //     // 패턴의 타입을 고정. -> Debug
    //     patternNum = (int)HurdleType.Bump;
    //     
    //     // 현재 진행 중인 Ground의 데이터를 가져온다.
    //     var ground = infiniteMap.GetGround();
    //     if (ground == null) return;
    //
    //     // 패턴의 위치 정보를 받아온다.
    //     var pattern = ground.GetPatternData(patternNum);
    //     // 장애물 스폰
    //     hurdleSpawner.SpawnHurdle(_spawnType, pattern);
    //
    //     PickRandType();
    // }
    //
    // private void SetCriteriaTime()
    // {
    //     if (_spawnTimer > 0)
    //         return;
    //     
    //     _spawnTimer = Random.Range(minTime, maxTime);
    // }
    //
    // private void SetSpawnType(HurdleType type)
    // {
    //     _spawnType = type;
    // }
    //
    // private void PickRandType()
    // {
    //     SetSpawnType((HurdleType)Random.Range(0, 3));
    // }
}