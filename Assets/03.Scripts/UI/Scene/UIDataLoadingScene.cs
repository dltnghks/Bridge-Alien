using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIDataLoadingScene : UIScene
{
    enum Objects
    {
        LoadProgressBar
    }

    enum Texts
    {
        LoadingText,
    }
    
    private Slider _progressBar;

    private float _endValue = 0;
    private float _curValue = 0;
    
    private float _animationSpeed = 1f;
    private Tween _loadingTween; // 현재 실행 중인 트윈

    public override bool Init()
    {
        if(base.Init() == false){
            return false;
        }

        BindObject(typeof(Objects));
        BindText(typeof(Texts));
        
        _progressBar = GetObject((int)Objects.LoadProgressBar).GetComponent<Slider>();
        _progressBar.value = 0;

        SetProgressBar(Managers.Data.TotalLoadCount);
        Managers.Data.OnDataLoaded += AddProgress;
        Managers.Data.OnAllDataLoaded += EndLoading;
        
        return true;
    }

    private void SetProgressBar(float endValue){
        _endValue = endValue;
        _progressBar.maxValue = endValue;
        _curValue = 0;
        _progressBar.minValue = 0;
    }

    private void AddProgress(Define.DataType dataType){
        Logger.Log($"[{dataType.ToString()}] Data Received");

        _curValue++;
        
        if (_loadingTween != null) _loadingTween.Kill(); // 기존 애니메이션 정지
        
        GetText((int)Texts.LoadingText).SetText($"Load {dataType.ToString()} Data...");
        _loadingTween = _progressBar.DOValue(_curValue, _animationSpeed).SetEase(Ease.OutQuad);
    }

    private void EndLoading()
    {   
        // 액션 해제
        Managers.Data.OnDataLoaded -= AddProgress;
        Managers.Data.OnAllDataLoaded -= EndLoading;

        // 데이터 로드 완료 시 Title Scene으로 변경
        if (_loadingTween != null)
        {
            _loadingTween.Kill();
            _progressBar.DOValue(_curValue, _animationSpeed).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                // 씬 전환
                Managers.Scene.ChangeScene(Define.Scene.Title);
            });
        }
    }
}
