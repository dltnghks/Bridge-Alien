using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
    private Define.Scene _curSceneType = Define.Scene.Unknown;
    
    public Define.Scene CurrentSceneType
    {
        get
        {
            if (_curSceneType != Define.Scene.Unknown)
                return _curSceneType;
            return CurrentScene.SceneType;
        }
        set {  _curSceneType = value; }
    }
    public BaseScene CurrentScene { get { return GameObject.Find("@Scene").GetComponent<BaseScene>(); } }

    public void Init()
    {
    }
    
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        char[] letters = name.ToLower().ToCharArray();
        letters[0] = char.ToUpper(letters[0]);
        return new string(letters);
    }
    
    public void ChangeScene(Define.Scene type)
    {
        // 현재 씬 클리어
        CurrentScene.Clear();
        _curSceneType = type;
        StartLoading();
    }
    
    private void StartLoading()
    {
        Managers.Fade.FadeIn(
            () =>
            {
                var loadingPopup = Managers.UI.ShowPopUI<UILoadingPopup>();
                var targetSceneName = GetSceneName(_curSceneType);
                
                Managers.Fade.FadeOut(
                    () =>
                    {
                        LoadScene(targetSceneName);
                    });
            });
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
