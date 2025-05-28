using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private List<MapObjectSpawner> _spawners = new List<MapObjectSpawner>();
    
    [Header("장애물 프리팹")]
    [SerializeField] private List<GameObject> obstacles = new List<GameObject>();
    
    public void SpawnObstacle(int index)
    {
        _spawners[index].CreateObject(obstacles[GetRandIndex()]);
    }
    
    public void RemoveObstacle(int index)
    {
        _spawners[index].RemoveObject();
    }

    private int GetRandIndex()
    {
        return Random.Range(0, obstacles.Count);
    }
}
