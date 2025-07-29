using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Ground : MonoBehaviour
{
    [SerializeField] private int pointCount = 0;
    [SerializeField] private Transform spawnParent;
    
    private Transform[] _spawnPoints;
    
    private List<GameObject> _hurdles = new List<GameObject>();

    public void Initialize()
    {
        _spawnPoints = new Transform[pointCount];

        for (int i = 0; i < pointCount; ++i)
            _spawnPoints[i] = spawnParent.transform.GetChild(i);
    }

    public Transform GetSpawnPoint(int idx)
    {
        idx = Mathf.Clamp(idx, 0, pointCount - 1);
        return _spawnPoints[idx];
    }

    public Transform[] GetPatternData(int type)
    {
        Transform[] pattern = new Transform[3];
        
        switch (type)
        {
            case 0:    // ZigZag - 0, 4, 2
                pattern[0] = GetSpawnPoint(0);
                pattern[1] = GetSpawnPoint(4);
                pattern[2] = GetSpawnPoint(2);
                break;
            
            case 1:    // ZigZag - 3, 1, 5
                pattern[0] = GetSpawnPoint(3);
                pattern[1] = GetSpawnPoint(1);
                pattern[2] = GetSpawnPoint(5);
                break;
            
            case 2:    // Center - 6
                pattern[0] = GetSpawnPoint(6);
                break;
        }
        return pattern;
    }
    
    public void ClearGround()
    {
        if (_hurdles.Count <= 0)
            return;

        foreach (var hurdle in _hurdles)
            Destroy(hurdle);
    }
}
