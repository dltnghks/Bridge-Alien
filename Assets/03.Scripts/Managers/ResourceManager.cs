using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ResourceManager
{
    public void Init()
    {
    }

    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefab/{path}");
        if (prefab == null)
        {
            Logger.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        
        Object.Destroy(go);
    }

    public void SetActive(GameObject go, bool value)
    {
        if (go == null)
            return;
        
        go.SetActive(value);
    }
}
