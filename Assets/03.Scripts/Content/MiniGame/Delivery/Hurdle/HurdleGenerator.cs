using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurdleGenerator : MonoBehaviour
{
    // HurdleGenerator는 게임 실행 시, 한 번에 모든 장애물을 생성한다.
    [field: SerializeField] public List<HurdlePattern> HurdlePattern { get; private set; }

    [Space(10)]
    [field: SerializeField] private float minRandWidth;
    [field: SerializeField] private float maxRandWidth;
    
    private GroundManager _groundManager;

    [SerializeField] private GameObject examplePrefab;
    
    private void Awake()
    {
        if (!TryGetComponent(out _groundManager))
        {
            Debug.Log("Ground Manager is Not Found");
            Debug.Break();
        }
    }

    private void Start()
    {
        SpawnHurdle();
    }

    private void SpawnHurdle()
    {
        var spawnStartWidth = _groundManager.GetGroundStartPosition().x;
        var spawnEndWidth = _groundManager.GetGroundEndPosition().x;
        
        var hurdlePattern = GetRandPick();

        Vector3 beforeSpawnPoint = new Vector3(spawnStartWidth, 3f, 0f);
        while (true)
        {
            // 장애물 생성 위치를 구한다.
            var spawnPoint = new Vector3(
                beforeSpawnPoint.x + GetOffset(),
                beforeSpawnPoint.y,
                beforeSpawnPoint.z
            );
            
            Debug.Log(spawnPoint + " : " + beforeSpawnPoint.x + " : " + spawnEndWidth);
            // 장애물 생성 위치가 끝보다 안쪽에 있는지 확인한다.
            if (spawnPoint.x < spawnEndWidth)
            {
                // 장애물 생성
                var hurdle = Instantiate(examplePrefab, spawnPoint, Quaternion.identity);
                // 장애물의 위치를 저장한다.
                beforeSpawnPoint = spawnPoint;
            }
            else
            {
                Debug.Log("Break");
                break;
            }
        }
    }

    private HurdlePattern GetRandPick()
    {
        return HurdlePattern[Random.Range(0, HurdlePattern.Count)];
    }

    private float GetOffset()
    {
        return Random.Range(minRandWidth, maxRandWidth);
    }
}
