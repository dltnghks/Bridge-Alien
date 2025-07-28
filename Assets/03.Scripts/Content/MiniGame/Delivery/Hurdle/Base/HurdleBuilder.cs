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

    public void Build(params float[] origins)
    {
        CreateEntry(origins[0]);

        if (objParent.TryGetComponent<MonoBehaviour>(out var runner))
        {
            runner.StartCoroutine(SpawnRoutine(origins));
        }

        CreateEnd(origins[0]);
    }

    private IEnumerator SpawnRoutine(float[] origins)
    {
        foreach (var origin in origins)
        {
            CreateMain(origin);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}