using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Managers : MonoBehaviour
{
    private static Managers _instance = null;
    public static Managers Instance
    {
        get { Init();  return _instance; }
    }

    private static ResourceManager _resourceManager = new ResourceManager();
    private static UIManager _uiManager = new UIManager();
    private static SceneManagerEx _sceneManager = new SceneManagerEx();
    private static FadeManager _fadeManager;
    
    public static ResourceManager Resource
    {
        get { Init(); return _resourceManager; }
    }
    
    public static UIManager UI
    {
        get{ Init(); return _uiManager; }
    }

    public static SceneManagerEx Scene
    {
        get{ Init(); return _sceneManager; }
    }
    
    public static FadeManager Fade
    {
        get{ Init(); return _fadeManager; }
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
            _fadeManager = Utils.GetOrAddComponent<FadeManager>(go);
            _fadeManager.Init();
            
            DontDestroyOnLoad(go);
        }
    }
}
