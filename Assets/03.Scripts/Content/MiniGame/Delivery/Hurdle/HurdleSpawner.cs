using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Transform uiParent;
    [SerializeField] private Transform objParent;
    
    [Header("장애물 S.O 데이터")]
    [SerializeField] private BumpBuilder bumpObject;
    [SerializeField] private GameObject carCrushObject;
    [SerializeField] private GameObject workObject;

    public void Initialize()
    {
        _builders = new Dictionary<HurdleType, HurdleBuilder>();
        
        _builders.Add(HurdleType.Bump, bumpObject);
        // _builders.Add(HurdleType.CarCrush, CarCrushObject.GetComponent<CarCrushBuilder>());
        // _builders.Add(HurdleType.Work, WorkObject.GetComponent<WorkBuilder>());

        // 등록된 Builder로 Parent 데이터를 넘겨준다.
        foreach (var build in _builders.Values)
            build.Initialize(uiParent, objParent);
        Debug.Log("Check");
    }

    public void SpawnHurdle(HurdleType type, params Vector3[] origins)
    {
        if (_builders.TryGetValue(type, out var builder))
        {
            builder.Build(origins);
        }
    }
}
