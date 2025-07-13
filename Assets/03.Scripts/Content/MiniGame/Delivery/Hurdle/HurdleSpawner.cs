using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Transform uiParent;
    [SerializeField] private Transform objParent;
    
    [Header("장애물 S.O 데이터")]
    [SerializeField] private BumpBuilder BumpObject;
    [SerializeField] private GameObject CarCrushObject;
    [SerializeField] private GameObject WorkObject;

    public void Initialize()
    {
        _builders.Add(HurdleType.Bump, BumpObject);
        // _builders.Add(HurdleType.CarCrush, CarCrushObject.GetComponent<CarCrushBuilder>());
        // _builders.Add(HurdleType.Work, WorkObject.GetComponent<WorkBuilder>());

        // 등록된 Builder로 Parent 데이터를 넘겨준다.
        foreach (var build in _builders.Values)
            build.Initialize(uiParent, objParent);
    }

    public void SpawnHurdle(HurdleType type, params Vector3[] origins)
    {
        if (_builders.TryGetValue(type, out var builder))
        {
            builder.Build(origins);
        }
    }
}
