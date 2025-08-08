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
    private static UIManager _uiManager;
    private static MiniGameManager  _miniGameManager;
    private static SceneManagerEx _sceneManager;
    private static FadeManager _fadeManager;
    private static CameraManager _cameraManager;
    private static SoundManager _soundManager;
    private static DeviceInputManager _deviceInputManager;
    private static PoolManager _poolManager = new PoolManager();
    private static DailyManager _dailyManager = new DailyManager();
    private static PlayerManager _playerManager = new PlayerManager();
    private static SaveManager _saveManager = new SaveManager();

    public static DataManager Data {get { Init(); return _dataManager;}}
    public static ResourceManager Resource { get { Init(); return _resourceManager; } }
    public static UIManager UI { get{ Init(); return _uiManager; } }
    public static SceneManagerEx Scene { get{ Init(); return _sceneManager; } }
    public static FadeManager Fade { get{ Init(); return _fadeManager; } }
    public static MiniGameManager MiniGame { get{ Init(); return _miniGameManager; } }
    public static CameraManager Camera{get{Init(); return _cameraManager; }}
    public static SoundManager Sound{get{Init(); return _soundManager; }}
    public static DeviceInputManager DeviceInput{get{ Init();  return _deviceInputManager; }}
    public static PoolManager Pool { get { Init(); return _poolManager; } }
    public static DailyManager Daily { get{ Init(); return _dailyManager; } }
    public static PlayerManager Player { get{ Init(); return _playerManager; } }
    public static SaveManager Save { get{ Init(); return _saveManager; } }
    
    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            _instance = Utils.GetOrAddComponent<Managers>(go);

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

            _deviceInputManager = Utils.GetOrAddComponent<DeviceInputManager>(go);
            _deviceInputManager.Init();

            _uiManager = Utils.GetOrAddComponent<UIManager>(go);
            _uiManager.Init();
        
            _resourceManager.Init();
            _poolManager.Init();
            _playerManager.Init();
            _dailyManager.Init();

            _saveManager.Init(new LocalFileStorage());
            _saveManager.Register(Player);
            _saveManager.Register(Daily);
            _saveManager.Register(MiniGame);
            
            DontDestroyOnLoad(go);
        }
    }
}
