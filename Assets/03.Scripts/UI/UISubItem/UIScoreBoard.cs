using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIScoreBoard : UISubItem
{
    enum Texts
    {
        ScoreText,
    }

    private int _curScore;
    
    private readonly float _textScaleDuration = 0.3f; // 텍스트 확대/축소에 걸리는 시간
    private readonly float _textScaleFactor = 1.2f;   // 텍스트가 확대될 비율
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));

        _curScore = 0;
        
        return true;
    }

    public void SetScore(int score)
    {
        _curScore = score;
        SetScoreText(_curScore);
    }

    public void AddScore(int score)
    {
        _curScore += score;
        SetScoreText(_curScore);
        
        // DOTween을 이용한 점수 텍스트 확대/축소 애니메이션
        var scoreText = GetText((int)Texts.ScoreText).transform;
        scoreText.DOScale(_textScaleFactor, _textScaleDuration)
            .SetEase(Ease.OutQuad)  // 부드러운 애니메이션을 위해 Ease 설정
            .OnComplete(() => scoreText.DOScale(1f, _textScaleDuration).SetEase(Ease.InQuad)); // 원래 크기로 복귀
    }

    private void SetScoreText(int score)
    {
        string scoreFormat = GetScoreFormat(score);
        GetText((int)Texts.ScoreText).SetText(scoreFormat);
    }

    private string GetScoreFormat(int score)
    {
        return $"Score : {score.ToString()}";
    }
}
