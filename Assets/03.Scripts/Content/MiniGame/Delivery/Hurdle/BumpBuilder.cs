using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bump", menuName = "Hurdle/Bump")]
public class BumpBuilder : HurdleBuilder
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject mainPrefab;
    
    public override GameObject CreateEntry(float yPos)
    {
        // return Instantiate(entryPrefab, Vector3.zero, Quaternion.identity, uiParent);
        return null;
    }
    public override GameObject CreateMain(float yPos)
    {
        var resultPosition = mainPrefab.transform.position;
        resultPosition.y = yPos;
        resultPosition.z = -2f;
        
        return Instantiate(mainPrefab, resultPosition, Quaternion.identity, objParent);
    }
    public override GameObject CreateEnd(float yPos)
    {
        return null;
    }
}