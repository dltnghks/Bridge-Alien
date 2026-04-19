using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public enum ScoreBoardFeedbackType
{
    Normal,
    Penalty,
    Combo,
    Lucky
}

public class UIScoreBoard : UISubItem
{
    private static readonly Color DefaultScoreColor = new Color32(0xB0, 0xD7, 0x9E, 0xFF);

    enum Texts
    {
        ScoreText,
    }

    private int _curScore;
    private Tween _feedbackTween;
    
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
        Init();
        _curScore = score;
        SetScoreText(_curScore);
    }

    private void SetScoreText(int score)
    {
        string scoreFormat = GetScoreFormat(score);
        GetText((int)Texts.ScoreText).SetText(scoreFormat);
    }

    public void PlayFeedback(ScoreBoardFeedbackType feedbackType)
    {
        Init();

        TextMeshProUGUI scoreLabel = GetText((int)Texts.ScoreText);
        Transform scoreTransform = scoreLabel.transform;
        scoreTransform.DOKill();
        scoreLabel.DOKill();
        _feedbackTween?.Kill();
        scoreTransform.localScale = Vector3.one;
        scoreLabel.color = DefaultScoreColor;

        float scale = 1.2f;
        float duration = 0.3f;

        switch (feedbackType)
        {
            case ScoreBoardFeedbackType.Penalty:
                scale = 1.08f;
                duration = 0.14f;
                break;
            case ScoreBoardFeedbackType.Combo:
                scale = 1.2f;
                duration = 0.3f;
                break;
            case ScoreBoardFeedbackType.Lucky:
                scale = 1.2f;
                duration = 0.3f;
                break;
        }

        _feedbackTween = scoreTransform.DOScale(scale, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                scoreTransform.DOScale(1f, duration).SetEase(Ease.InQuad);
            })
            .OnKill(() =>
            {
                scoreTransform.localScale = Vector3.one;
                scoreLabel.color = DefaultScoreColor;
            });
    }

    private string GetScoreFormat(int score)
    {
        return $"{score}";
    }
}
