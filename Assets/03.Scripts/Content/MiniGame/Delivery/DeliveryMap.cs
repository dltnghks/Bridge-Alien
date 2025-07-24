using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMap : MonoBehaviour
{
    [SerializeField] private HurdleSpawner hurdleSpawner;
    [SerializeField] private InfiniteMap infiniteMap;

    private float _spawnTimer = .0f;
    private HurdleType _spawnType;

    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    
    private bool isInitialized = false;

    public void Initialize(MapPacket packet)
    {
        hurdleSpawner.Initialize();
        infiniteMap.Initialize(packet.maxDist, packet.onUpdateDistance);
        
        // 랜덤으로 시간을 정하고 타입을 정한다.
        SetCriteriaTime();
        PickRandType();
        
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            SpawnOnMap();
        }
    }

    private void SpawnOnMap()
    {
        SetCriteriaTime();

        // 패턴 정보를 넘겨준다.
        int patternNum = (int)_spawnType;
        
        // 패턴의 타입을 고정. -> Debug
        patternNum = (int)HurdleType.Bump;
        
        // 현재 진행 중인 Ground의 데이터를 가져온다.
        var ground = infiniteMap.GetGround();
        if (ground == null) return;

        // 패턴의 위치 정보를 받아온다.
        var pattern = ground.GetPatternData(patternNum);
        // 장애물 스폰
        hurdleSpawner.SpawnHurdle(_spawnType, pattern);

        PickRandType();
    }

    private void SetCriteriaTime()
    {
        if (_spawnTimer > 0)
            return;
        
        _spawnTimer = Random.Range(minTime, maxTime);
    }

    private void SetSpawnType(HurdleType type)
    {
        _spawnType = type;
    }

    private void PickRandType()
    {
        SetSpawnType((HurdleType)Random.Range(0, 3));
    }
}