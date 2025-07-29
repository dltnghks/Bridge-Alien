using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class HurdleBuilder : ScriptableObject
{
    protected Transform uiParent;
    protected Transform objParent;

    [SerializeField] protected float spawnDelay = 2f;

    public abstract GameObject CreateEntry(float yPos);
    public abstract GameObject CreateMain(float yPos);
    public abstract GameObject CreateEnd(float yPos);

    public void Initialize(Transform uiParent, Transform objParent)
    {
        this.uiParent = uiParent;
        this.objParent = objParent;
    }

    public IEnumerator BuildRoutine(float[] origins)
    {
        CreateEntry(origins[0]);
        // yield로 시간의 흐름을 조정하기.

        foreach (var origin in origins)
        {
            CreateMain(origin);
            yield return new WaitForSeconds(spawnDelay);
        }

        CreateEnd(origins[0]);
    }
}