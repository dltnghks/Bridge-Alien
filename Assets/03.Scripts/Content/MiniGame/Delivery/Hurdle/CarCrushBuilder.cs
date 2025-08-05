using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CarCrush", menuName = "Hurdle/CarCrush")]
public class CarCrushBuilder : HurdleBuilder
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
}
