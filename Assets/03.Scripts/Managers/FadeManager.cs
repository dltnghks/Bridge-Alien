using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeManager : MonoBehaviour
{
    private GameObject _fadeObj;
    private CanvasGroup _canvasGroup;
    private float _fadeDuration = 0.5f;
    private UnityAction _onEndEvent;
    
    public void Init()
    {
        if (_fadeObj == null)
        {
            _fadeObj = Managers.Resource.Instantiate("FadeCanvas", Managers.Instance.transform);
        }
        
        if (_canvasGroup == null)
        {
            _canvasGroup = _fadeObj.AddComponent<CanvasGroup>();
        }
        _canvasGroup.alpha = 0; 
    }
    
    // 페이드 인
    public void FadeIn(UnityAction onEndEvent = null)
    {
        Init();
        _onEndEvent = onEndEvent;
        _canvasGroup.alpha = 1;
        StartCoroutine(FadeRoutine(0)); // 밝아지게
    }

    // 페이드 아웃
    public void FadeOut(UnityAction onEndEvent = null)
    {
        Init();
        _onEndEvent = onEndEvent;
        _canvasGroup.alpha = 0;
        StartCoroutine(FadeRoutine(1)); // 어두워지게
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha; // 페이드 완료 시 최종 알파값 설정
        _onEndEvent?.Invoke();
    }
}
