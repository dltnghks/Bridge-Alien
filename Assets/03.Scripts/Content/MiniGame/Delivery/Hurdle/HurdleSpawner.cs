using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public enum HurdleType
{
    Bump,
    CarCrush,
    Work
}

public class HurdleSpawner : MonoBehaviour
{
    private Dictionary<HurdleType, HurdleBuilder> _builders;

    [Header("장애물 생성 필요 값")]
    [SerializeField] private TMP_Text textObj;
    [SerializeField] private HurdleObjectManager hurdleObjectManager;

    [Header("장애물 생성 관련 수치")]
    [SerializeField] private float spawnInterval;

    private Transform _objParent;
    private Transform _uiParent;

    [Header("장애물 S.O 데이터")]
    [SerializeField] private BumpBuilder bumpObject;
    [SerializeField] private CarCrushBuilder carCrushObject;
    [SerializeField] private WorkBuilder workObject;

    private float _timer = 0f;
    private bool _isRunning = false;
    
    // Debug
    private HurdleType _spawnType = HurdleType.Bump;

    private readonly float[] yPositions = new float[3] { -9.5f, -4.2f, -7.7f };

    public void Initialize()
    {
        _builders = new Dictionary<HurdleType, HurdleBuilder>
        {
            { HurdleType.Bump, bumpObject },
            { HurdleType.CarCrush, carCrushObject},
            { HurdleType.Work, workObject }
        };

        _objParent = hurdleObjectManager.transform;
        _uiParent = textObj.transform;

        foreach (var build in _builders.Values)
            build.Initialize(_uiParent, _objParent);

        hurdleObjectManager.Initialize(5f);
        _isRunning = true;
    }

    private void Update()
    {
        if (!_isRunning) return;

        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            SpawnHurdle(_spawnType); // 자동 생성
        }
    }

    public void ChangeTime(float seconds)
    {
        spawnInterval = Mathf.Max(0.1f, seconds); // 최소 0.1초 이상
    }

    public HurdleType GetHurdleType()
    {
        return (HurdleType)Random.Range(0, 3);
    }

    public void StopSpawning()
    {
        _isRunning = false;
    }

    public void ResumeSpawning()
    {
        _isRunning = true;
    }

    public void SpawnHurdle(HurdleType type)
    {
        _spawnType = GetHurdleType();
        SpawnHurdle(_spawnType, GetOrigins());
    }

    private void SpawnHurdle(HurdleType type, params float[] origins)
    {
        if (_builders.TryGetValue(type, out var builder))
        {
            builder.Build(origins);
        }
    }

    private float[] GetOrigins()
    {
        List<float> origins = new List<float>();

        switch (_spawnType)
        {
            case HurdleType.Bump:
                origins.Add(yPositions[2]); 
                break;

            case HurdleType.Work:
            case HurdleType.CarCrush:
                int count = Random.Range(3, 7);

                for (int i = 0; i < count; i++)
                {
                    float y = yPositions[i % 2];
                    origins.Add(y);
                }
                
                Debug.Log(origins);
                
                break;
        }

        return origins.ToArray();
    }
}
