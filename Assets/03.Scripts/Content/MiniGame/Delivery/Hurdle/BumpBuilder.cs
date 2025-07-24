using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bump", menuName = "Hurdle")]
public class BumpBuilder : HurdleBuilder
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject mainPrefab;
    
    public override GameObject CreateEntry(Transform position)
    {
        return Instantiate(entryPrefab, Vector3.zero, Quaternion.identity, uiParent);
    }
    public override GameObject CreateMain(Transform position)
    {
        return Instantiate(mainPrefab, position.position, Quaternion.identity, objParent);
    }
    public override GameObject CreateEnd(Transform position)
    {
        return null;
    }
}
