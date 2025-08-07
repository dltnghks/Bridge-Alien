using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using AYellowpaper.SerializedCollections;
using Random = UnityEngine.Random;

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
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;

    [Header("장애물 S.O 데이터")]
    [SerializeField] private BumpBuilder bumpObject;
    [SerializeField] private CarCrushBuilder carCrushObject;
    [SerializeField] private WorkBuilder workObject;

    private bool _isRunning = false;

    private Transform _objParent;
    private Transform _uiParent;

    private MiniGameDelivery _miniGame;

    private Coroutine _currentStartCoroutine;
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
    }

    public void StartHurdleSpawn()
    {
        if(_currentStartCoroutine == null && _miniGame.IsActive && _miniGame.IsPause == false)
            _currentStartCoroutine = StartCoroutine(HurdleSpawnLoop());
    }

    public void ChangeGroundSpeed(float speed)
    {
        hurdleObjectManager.ChangeSpeed(speed);
    }

    private bool _wasPaused = false;
    private bool _hasSpawnStarted = false;
    
    private void Update()
    {
        // 장애물 스폰이 진행이 안되고 있거나, 미니게임이 없다면
        if (!_isRunning || _miniGame == null) return;

        if (!_miniGame.IsPause && _wasPaused && !_hasSpawnStarted)
        {
            StartHurdleSpawn();
            _hasSpawnStarted = true;
        }

        _wasPaused = _miniGame.IsPause;

        if (_miniGame.IsPause || _miniGame.IsActive == false)
        {
            if (_currentStartCoroutine != null)
            {
                StopCoroutine(_currentStartCoroutine);
                _currentStartCoroutine = null;
            }

            _hasSpawnStarted = false;
        }
    }

    public void ClearHurdles()
    {
        if (hurdleObjectManager == null)
            return;

        Transform parent = hurdleObjectManager.transform;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }


    private IEnumerator HurdleSpawnLoop()
    {
        while (_isRunning)
        {
            yield return new WaitForSeconds(GetRandTime());

            var type = GetHurdleType();
            var origins = GetOrigins(type);

            if (builders.TryGetValue(type, out var builder) && _miniGame.IsPause == false && _miniGame.IsActive && _miniGame.IsHurdleSpawnFlag)
            {
                _currentBuildCoroutine = StartCoroutine(builder.BuildRoutine(origins));
                yield return _currentBuildCoroutine;
                _currentBuildCoroutine = null;
            }
            else if(_currentBuildCoroutine != null)
            {
                StopCoroutine(_currentBuildCoroutine);
                _currentBuildCoroutine = null;
            }
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

    private float GetRandTime()
    {
        var resultTime = Random.Range(minTime, maxTime);
        return resultTime;
    }
    
}
