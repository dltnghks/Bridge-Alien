using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.Profiling;
using System;

public class UIGameStartPopup : UIPopup
{
    enum Images{
        GameStartImage,
    }

    private Image _gameStartImage;
    public float ScaleDuration = 1f; // 이미지가 커지는 시간
    public float FadeDuration = 1f;  // 이미지가 투명해지는 시간
    public float HoldDuration = 0.5f; // 이미지가 유지되는 시간


    public override bool Init(){
        if(base.Init() == false){
            return false;
        }

        BindImage(typeof(Images));

        _gameStartImage = GetImage((int)Images.GameStartImage);

        Managers.UI.SetInputBackground(false);
        return true;
    }

    public void PlayGameStartEffect(Action gameStartAction = null)
    {
        Init();
        Debug.Log("Init Pass");

        // 초기 상태 설정
        _gameStartImage.transform.localScale = Vector3.zero; // 이미지 크기를 0으로 설정
        _gameStartImage.color = new Color(1, 1, 1, 1);       // 불투명한 상태로 설정

        // DOTween Sequence 생성
        Sequence sequence = DOTween.Sequence();

        // 1. 이미지를 키우기
        sequence.Append(_gameStartImage.transform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack));
        Debug.Log("StartPopup - 1 - Pass");

        // 2. 잠시 유지
        sequence.AppendInterval(HoldDuration);
        Debug.Log("StartPopup - 2 - Pass");

        // 3. 투명해지기 (페이드 아웃)
        sequence.Append(_gameStartImage.DOFade(0, FadeDuration).SetEase(Ease.InQuad));
        Debug.Log("StartPopup - 3 - Pass");

        // 4. 애니메이션 종료 후 이미지 비활성화
        sequence.OnComplete(() => 
        {
            Debug.Log("Sequence Before");
            gameStartAction?.Invoke();
            ClosePopupUI();
            Debug.Log("Sequence Ended");
        });
        
        Debug.Log("StartPopup - All - Pass");
    }

    public override void ClosePopupUI()
    {
        Managers.UI.SetInputBackground(true);
        base.ClosePopupUI();
    }

}
