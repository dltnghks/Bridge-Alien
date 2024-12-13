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
    private static MiniGameManager  _miniGameManager;
    private static SceneManagerEx _sceneManager;
    private static FadeManager _fadeManager;
    private static CameraManager _cameraManager;
    private static SoundManager _wwiseSoundBankManager;
    
    
    public static ResourceManager Resource { get { Init(); return _resourceManager; } }
    public static UIManager UI { get{ Init(); return _uiManager; } }
    public static SceneManagerEx Scene { get{ Init(); return _sceneManager; } }
    public static FadeManager Fade { get{ Init(); return _fadeManager; } }
    public static MiniGameManager MiniGame { get{ Init(); return _miniGameManager; } }
    public static CameraManager Camera{get{Init(); return _cameraManager; }}
    public static SoundManager SoundBank{get{Init(); return _wwiseSoundBankManager; }}

    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            _instance = Utils.GetOrAddComponent<Managers>(go);
            
            _resourceManager.Init();

            _sceneManager = Utils.GetOrAddComponent<SceneManagerEx>(go);
            _sceneManager.Init();
            
            _fadeManager = Utils.GetOrAddComponent<FadeManager>(go);
            _fadeManager.Init();
            
            _miniGameManager = Utils.GetOrAddComponent<MiniGameManager>(go);
            _miniGameManager.Init();

            _cameraManager = Utils.GetOrAddComponent<CameraManager>(go);
            _cameraManager.Init();

            _wwiseSoundBankManager = Utils.GetOrAddComponent<SoundManager>(go);
            _wwiseSoundBankManager.Init();

            
            DontDestroyOnLoad(go);
        }
    }
}
