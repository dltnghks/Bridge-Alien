using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance = null;
    public static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }

    private ResourceManager _resourceManager = new ResourceManager();
    private UIManager _uiManager;

    private void Start()
    {
        Debug.Log("ddd");
    }

    public static ResourceManager Resource
    {
        get { return Instance._resourceManager; }
    }
    
    public static UIManager UI
    {
        get{ return Instance._uiManager;}
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            _instance = Utils.GetOrAddComponent<Managers>(go);
            
            DontDestroyOnLoad(go);
        }
    }
}
