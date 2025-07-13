using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bump", menuName = "Hurdle")]
public class BumpBuilder : HurdleBuilder
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject mainPrefab;
    
    public override GameObject CreateEntry(Vector3 position)
    {
        return Instantiate(entryPrefab, Vector3.zero, Quaternion.identity, uiParent);
    }
    public override GameObject CreateMain(Vector3 position)
    {
        return Instantiate(mainPrefab, position, Quaternion.identity, objParent);
    }
    public override GameObject CreateEnd(Vector3 position)
    {
        return null;
    }
}
