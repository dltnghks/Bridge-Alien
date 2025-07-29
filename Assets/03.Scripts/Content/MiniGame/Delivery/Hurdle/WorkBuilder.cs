using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Work", menuName = "Hurdle/Work")]
public class WorkBuilder : HurdleBuilder
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private GameObject mainPrefab;
    [SerializeField] private GameObject endPrefab;

    public override GameObject CreateEntry(float yPos)
    {
        return null;
        // return Instantiate(entryPrefab, Vector3.zero, Quaternion.identity, uiParent);
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
