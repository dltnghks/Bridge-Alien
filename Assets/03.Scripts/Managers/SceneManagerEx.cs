using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneManagerEx : MonoBehaviour
{
    private Define.Scene _curSceneType = Define.Scene.Unknown;
    private Define.Scene _selectedSceneType = Define.Scene.MiniGameUnload;
    public event UnityAction<string> SelectedSceneAction;
    private float _minimumLoadTime = 2.0f;
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

    public Define.Scene SelectedSceneType
    {
        get { return _selectedSceneType; }
        set 
        {        
            Logger.Log("Manager change selected Scene Type");
            _selectedSceneType = value;
            SelectedSceneAction?.Invoke(GetSceneName(_selectedSceneType));
        }
    }

    public BaseScene CurrentScene { get { return GameObject.Find("@Scene").GetComponent<BaseScene>(); } }

    public void Init()
    {
    }
    
    string GetSceneName(Define.Scene type)
    {
        string name = System.Enum.GetName(typeof(Define.Scene), type);
        return new string(name);
    }
    
    public void ChangeScene(Define.Scene type)
    {
        if(type == Define.Scene.Unknown){
            Logger.LogWarning("정의되지 않은 씬 타입입니다.");
            return;
        }
        // 현재 씬 클리어
        CurrentScene.Clear();
        _curSceneType = type;
        StartLoading();
    }

    public void ChangeSelectedScene()
    {
        if(SelectedSceneType == Define.Scene.Unknown){
            Logger.LogWarning("정의되지 않은 씬 타입입니다.");
            return;
        }
        // 현재 씬 클리어
        CurrentScene.Clear();
        _curSceneType = SelectedSceneType;
        StartLoading();
    }
    
    private void StartLoading()
    {
        // 페이드 아웃 시작
        Managers.Fade.FadeOut(() =>
        {
            var targetSceneName = GetSceneName(_curSceneType);
        
            // 비동기 씬 로드와 최소 로딩 시간 유지
            LoadSceneAsync(targetSceneName, () =>
            {
                // 페이드 인 시작
                Managers.Fade.FadeIn();
            });
        });
    }

    private void LoadSceneAsync(string sceneName, UnityAction onSceneLoaded)
    {
        // 비동기 씬 로드 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 씬 활성화를 보류하여 대기 상태로 유지
        
        Managers.Fade.FadeIn();
        // 로딩 팝업 표시
        var loadingPopup = Managers.UI.ShowPopUI<UILoadingPopup>();

        // 최소 로딩 시간과 비동기 씬 로드 완료 상태를 함께 기다림
        StartCoroutine(WaitForMinimumLoadTime(asyncLoad, _minimumLoadTime, () =>
        {
            // 씬 활성화 허용 및 페이드 인 시작
            asyncLoad.allowSceneActivation = true;            
            onSceneLoaded?.Invoke();
            // 로딩 팝업 닫기
            Managers.UI.ClosePopupUI(loadingPopup);

        }));
    }

    private IEnumerator WaitForMinimumLoadTime(AsyncOperation asyncLoad, float minimumLoadTime, UnityAction onComplete)
    {
        // 최소 로딩 시간 대기
        float elapsedTime = 0f;
        while (elapsedTime < minimumLoadTime || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 로딩이 완료되었고 최소 시간이 경과한 경우 콜백 실행
        onComplete?.Invoke();
    }
}