using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class HurdleBuilder : ScriptableObject
{
    protected Transform uiParent;
    protected Transform objParent;

    [SerializeField] protected float spawnDelay = 0.5f;

    public abstract GameObject CreateEntry(float yPos);
    public abstract GameObject CreateMain(float yPos);

    public void Initialize(Transform ui, Transform obj)
    {
        uiParent = ui;
        objParent = obj;
    }

    public IEnumerator BuildRoutine(float[] origins)
    {
        CreateEntry(origins[0]);

        GameObject mainObject = null;
        Debug.Log("Spawn 시작 ");
        foreach (var origin in origins)
        {
            if (Managers.MiniGame.CurrentGame.IsPause == false)
            {
                mainObject = CreateMain(origin);
            }

            if(mainObject != null)
                yield return new WaitForSeconds(spawnDelay);
        }
        Debug.Log("Spawn 종료 ");
    }
}