using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance = null;
    public static Managers Instance
    {
        get { Init();  return _instance; }
    }

    private static ResourceManager _resourceManager = new ResourceManager();
    private static UIManager _uiManager = new UIManager();
    
    public static ResourceManager Resource
    {
        get { Init(); return _resourceManager; }
    }
    
    public static UIManager UI
    {
        get{ Init(); return _uiManager;}
    }

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            _instance = Utils.GetOrAddComponent<Managers>(go);
            
            _resourceManager.Init();
            
            DontDestroyOnLoad(go);
        }
    }
}
