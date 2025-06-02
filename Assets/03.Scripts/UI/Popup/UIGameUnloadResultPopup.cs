using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIGameUnloadResultPopup : UIConfirmPopup
{
    enum Texts
    {
        ScoreText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        
        Managers.UI.SetInputBackground(false);
        
        return true;
    }

    public void SetResultScore(int score)
    {
        //gameObject.transform.DOScale(Vector3.one, 2);
        if (Init())
        {
            string scoreText = $"Score : {score.ToString()}";
            GetText((int)Texts.ScoreText).SetText(scoreText);
            ShowResultPopupEffect();
        }
    }

    protected override void OnClickConfirmButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Daily.EndMiniGameEvent();
    } 
    
    float ScaleDuration = 0.5f;
    float HoldDuration = 0.5f;
    
    public void ShowResultPopupEffect()
    {
        // 초기 상태 설정
        transform.localScale = Vector3.zero;
    
        transform.DOScale(Vector3.one, ScaleDuration);

        // DOTween Sequence 생성
        Sequence sequence = DOTween.Sequence();

        // 1. 이미지를 키우기
        sequence.Append(transform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.OutBack));

        // 2. 잠시 유지
        sequence.AppendInterval(HoldDuration);

        // 3. 투명해지기 (페이드 아웃)
        sequence.Append(transform.DOScale(Vector3.one, ScaleDuration).SetEase(Ease.InQuad));

    }       
    
    public override void ClosePopupUI()
    {
        Managers.UI.SetInputBackground(true);
        base.ClosePopupUI();       
    }
}
