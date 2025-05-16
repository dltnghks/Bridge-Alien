using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectSpawner : MonoBehaviour
{
    private GameObject _createObject;
    
    public void CreateObject(GameObject prefab)
    {
        if (prefab == null || _createObject != null) return;

        _createObject = Instantiate(prefab);
        _createObject.transform.position = transform.position;
        _createObject.transform.rotation = Quaternion.identity;
        _createObject.transform.localScale = new Vector3(1, 1, 1);
    }

    public void RemoveObject()
    {
        if(_createObject == null) return;
        
        Destroy(_createObject);
    }
}
