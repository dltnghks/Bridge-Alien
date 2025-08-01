using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using AYellowpaper.SerializedCollections;

public enum HurdleType
{
    Bump,
    CarCrush,
    Work
}

public class HurdleSpawner : MonoBehaviour
{
    [SerializeField] private SerializedDictionary<HurdleType, HurdleBuilder> builders;

    [Header("장애물 생성 부모")]
    [SerializeField] private TMP_Text textObj;
    [SerializeField] private HurdleObjectManager hurdleObjectManager;

    [Header("장애물 생성 수치")]
    [SerializeField] private float spawnInterval;

    [Header("장애물 S.O 데이터")]
    [SerializeField] private BumpBuilder bumpObject;
    [SerializeField] private CarCrushBuilder carCrushObject;
    [SerializeField] private WorkBuilder workObject;

    private float _timer = 0f;
    private bool _isRunning = false;

    private Transform _objParent;
    private Transform _uiParent;

    private MiniGameDelivery _miniGame;
    
    private Coroutine _currentBuildCoroutine;

    private readonly float[] yPositions = new float[3] { -9.5f, -4.2f, -7.7f };

    public void Initialize(float groundSpeed)
    {
        _objParent = hurdleObjectManager.transform;
        _uiParent = textObj.transform;
        _miniGame = (MiniGameDelivery)Managers.MiniGame.CurrentGame;

        foreach (var build in builders.Values)
            build.Initialize(_uiParent, _objParent);

        hurdleObjectManager.Initialize(groundSpeed);

        _isRunning = true;

        StartCoroutine(HurdleSpawnLoop());
    }

    public void ChangeGroundSpeed(float speed)
    {
        hurdleObjectManager.ChangeSpeed(speed);
    }

    private IEnumerator HurdleSpawnLoop()
    {
        while (_isRunning)
        {
            if (_miniGame.IsHurdleSpawn && !_miniGame.IsActive)
            {
                Debug.Log("기준을 넘어서 스폰 실패.");

                if (_currentBuildCoroutine != null)
                {
                    StopCoroutine(_currentBuildCoroutine);
                    _currentBuildCoroutine = null;
                }

                yield break;
            }

            var type = GetHurdleType();
            var origins = GetOrigins(type);

            if (builders.TryGetValue(type, out var builder))
            {
                _currentBuildCoroutine = StartCoroutine(builder.BuildRoutine(origins));
                yield return _currentBuildCoroutine;
                _currentBuildCoroutine = null;
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public HurdleType GetHurdleType()
    {
        return (HurdleType)Random.Range(0, 3);
    }

    private float[] GetOrigins(HurdleType type)
    {
        List<float> origins = new List<float>();

        switch (type)
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
                break;
        }
        return origins.ToArray();
    }
}
