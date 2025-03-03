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
    
    private static DataManager _dataManager;
    private static ResourceManager _resourceManager = new ResourceManager();
    private static UIManager _uiManager = new UIManager();
    private static MiniGameManager  _miniGameManager;
    private static SceneManagerEx _sceneManager;
    private static FadeManager _fadeManager;
    private static CameraManager _cameraManager;
    private static SoundManager _soundManager;
    private static PoolManager _poolManager = new PoolManager();    
    
    public static DataManager Data {get { Init(); return _dataManager;}}
    public static ResourceManager Resource { get { Init(); return _resourceManager; } }
    public static UIManager UI { get{ Init(); return _uiManager; } }
    public static SceneManagerEx Scene { get{ Init(); return _sceneManager; } }
    public static FadeManager Fade { get{ Init(); return _fadeManager; } }
    public static MiniGameManager MiniGame { get{ Init(); return _miniGameManager; } }
    public static CameraManager Camera{get{Init(); return _cameraManager; }}
    public static SoundManager Sound{get{Init(); return _soundManager; }}
    
    public static PoolManager Pool { get{ Init(); return _poolManager; } }
    
    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            _instance = Utils.GetOrAddComponent<Managers>(go);

            _resourceManager.Init();
            _poolManager.Init();

            _dataManager = Utils.GetOrAddComponent<DataManager>(go);
            _dataManager.Init();
            
            _sceneManager = Utils.GetOrAddComponent<SceneManagerEx>(go);
            _sceneManager.Init();
            
            _fadeManager = Utils.GetOrAddComponent<FadeManager>(go);
            _fadeManager.Init();
            
            _miniGameManager = Utils.GetOrAddComponent<MiniGameManager>(go);
            _miniGameManager.Init();

            _cameraManager = Utils.GetOrAddComponent<CameraManager>(go);

            _soundManager = Utils.GetOrAddComponent<SoundManager>(go);
            _soundManager.Init();

            
            DontDestroyOnLoad(go);
        }
    }
}
