using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIGameUnloadResultPopup : UIConfirmPopup
{
    enum Texts
    {
        ScoreText,              // 게임 점수
        MiniGameTypeText,       // 미니게임 종류
        WorkerNameText,         // 이름
        MinimumWageText,        // 최저임금
        ScoreBonusText,         // 점수 보너스
        StatsBonusText,         // 스탯 보너스
        FatiguePenaltyText,     // 피로도 페널티
        TotalText,              // 총합
    }

    private float _scaleDuration = 0.5f;
    private float _holdDuration = 0.5f;

    [Header("Settings")]
    [SerializeField]
    private float _textDelayDuration = 1.5f;
    private bool _isFinished = false;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));


        return true;
    }

    protected override void OnClickConfirmButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Daily.EndMiniGameEvent();
    }


    public void SetResultScore(int score, int minimumWage, float experienceBonus, float fatiguePenalty, float scoreBonus, float totalScore)
    {
        //gameObject.transform.DOScale(Vector3.one, 2);
        if (Init())
        {
            // 고정 텍스트 설정
            SetFixedTexts(minimumWage);

            SetReceiptText(score, experienceBonus, fatiguePenalty, scoreBonus, totalScore);
            ShowResultPopupEffect();
        }
    }

    private void SetReceiptText(int score, float experienceBonus, float fatiguePenalty, float scoreBonus, float totalScore)
    {
        // 텍스트 값 설정
        GetText((int)Texts.ScoreText).SetText(score.ToString());
        GetText((int)Texts.ScoreBonusText).SetText($"<color=#4B95DA>(+)</color>{scoreBonus:0}%");
        GetText((int)Texts.StatsBonusText).SetText($"<color=#4B95DA>(+)</color>{experienceBonus:0}%");
        GetText((int)Texts.FatiguePenaltyText).SetText($"<color=#C62E2E>(-)</color>{fatiguePenalty:0}%");

        // 모든 텍스트 투명하게 초기화
        var textIndices = new[] {
            (int)Texts.ScoreText,
            (int)Texts.ScoreBonusText,
            (int)Texts.StatsBonusText,
            (int)Texts.FatiguePenaltyText,
            (int)Texts.TotalText,
        };

        foreach (var idx in textIndices)
            GetText(idx).DOFade(0f, 0f);

        // 순차적으로 Fade In
        Sequence sequence = DOTween.Sequence();
        foreach (var idx in textIndices)
        {
            sequence.AppendInterval(_textDelayDuration);
            sequence.Append(
            GetText(idx).DOFade(1f, 0f).SetEase(Ease.InQuad).OnComplete(() =>
                {
                    Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString(), gameObject);
                })
            );
        }

        sequence.Append(DOVirtual.Int(0, (int)totalScore, 2f, value =>
        {
            GetText((int)Texts.TotalText).SetText(value.ToString());
        }).SetEase(Ease.InQuad));

        sequence.Play().OnComplete(() =>
        {
            _isFinished = true;
            Managers.UI.SetInputBackground(true);
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.LastScore.ToString(), gameObject);
        });
    }

    private void SetFixedTexts(int minimumWage)
    {
        if (Managers.MiniGame.CurrentGame is MiniGameDelivery)
        {
            GetText((int)Texts.MiniGameTypeText).SetText("배송");
        }
        else if (Managers.MiniGame.CurrentGame is MiniGameUnload)
        {
            GetText((int)Texts.MiniGameTypeText).SetText("하차");
        }
        else
        {
            GetText((int)Texts.MiniGameTypeText).SetText("Unknown Game");
        }

        GetText((int)Texts.WorkerNameText).SetText("김이민");
        GetText((int)Texts.MinimumWageText).SetText(minimumWage.ToString());
    }

    public void ShowResultPopupEffect()
    {
        // 초기 상태 설정
        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, _scaleDuration);

        // DOTween Sequence 생성
        Sequence sequence = DOTween.Sequence();

        // 1. 이미지를 키우기
        sequence.Append(transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.OutBack));

        // 2. 잠시 유지
        sequence.AppendInterval(_holdDuration);

        // 3. 투명해지기 (페이드 아웃)
        sequence.Append(transform.DOScale(Vector3.one, _scaleDuration).SetEase(Ease.InQuad));

    }


    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }

    public void OnDestroy()
    {
        // 1차 시연때는 다음 이벤트로 넘어가기 X
        OnClickConfirmButton();
    }
}
