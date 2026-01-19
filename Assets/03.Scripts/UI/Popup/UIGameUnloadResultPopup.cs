using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class UIGameUnloadResultPopup : UIConfirmPopup
{
    enum Texts
    {
        ScoreText,              // 게임 점수
        MiniGameTypeText,       // 미니게임 종류
        WorkerNameText,         // 이름
        TotalGoldText,              // 총합
        Star1ScoreText,
        Star2ScoreText,
        Star3ScoreText,
        ClearRewardText,
        MinimumWageText,
        ExperimentsBonusText,
        ScoreBonus,
    }

    enum Images
    {
        Star1Icon,
        Star2Icon,
        Star3Icon,
        ClearIconImage,
    }

    private float _scaleDuration = 0.5f;
    private float _holdDuration = 0.5f;

    [Header("Settings")]
    [SerializeField]
    private float _textDelayDuration = 1.5f;
    private bool _isFinished = false;
    private bool _isEventEnded = false;
    private int _starCount = 0;

    private List<UIActiveButton> _stars = new List<UIActiveButton>();

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindImage(typeof(Images));

        for (int i = (int)Images.Star1Icon; i <= (int)Images.Star3Icon; i++)
        {
            var star = GetImage(i).gameObject;
            if (star is null)
            {
                Logger.LogError($"{i}Star Icon is null"); 
                continue;
            }
            star.GetOrAddComponent<UIActiveButton>().Deactivate();
            _stars.Add(star.GetOrAddComponent<UIActiveButton>());
        }


        var textIndices = new[] {
            (int)Texts.ScoreText,
            (int)Texts.TotalGoldText,
            (int)Texts.MinimumWageText,
            (int)Texts.ExperimentsBonusText,
            (int)Texts.ScoreBonus,
        };

        foreach (var idx in textIndices)
            GetText(idx).color = Color.clear;

        GetImage((int)Images.ClearIconImage).color = Color.clear;

        return true;
    }

    protected override void OnClickConfirmButton()
    {
        if (_isEventEnded) return;
        _isEventEnded = true;

        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        FinishGame();
    }

    private void FinishGame()
    {
        Managers.Stage.EndStage(_starCount);
    }


    public void SetResultScore(int score, int totalGold, int preStarCount, int starCount, int[] scoreList, int clearReward, int minimumWage, int scoreBonus, int statsBonus)
    {
        _starCount = starCount;

        SetPreStar(preStarCount);
        SetReceiptText(scoreList, clearReward);
        SetBonusValues(minimumWage, statsBonus, scoreBonus);

        ShowResultPopupEffect();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(ShowScore(score + statsBonus));
        sequence.AppendInterval(0.2f);

        sequence.Append(ShowStar(starCount));
        sequence.AppendInterval(0.2f);

        sequence.Append(ShowBonusTexts());
        sequence.AppendInterval(0.2f);

        sequence.Append(ShowTotalGold(totalGold));

        sequence.Play().OnComplete(() =>
        {
            _isFinished = true;
            Managers.UI.SetInputBackground(true);
            Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.LastScore.ToString(), gameObject);
        });
    }

    private void SetBonusValues(int minimumWage, int experimentsBonus, int scoreBonus)
    {
        GetText((int)Texts.MinimumWageText).SetText(minimumWage.ToString());
        GetText((int)Texts.ExperimentsBonusText).SetText(experimentsBonus.ToString());
        GetText((int)Texts.ScoreBonus).SetText(scoreBonus.ToString());
    }

    private Sequence ShowBonusTexts()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => GetText((int)Texts.MinimumWageText).color = Color.black);
        sequence.AppendInterval(0.2f);
        sequence.AppendCallback(() => GetText((int)Texts.ExperimentsBonusText).color = Color.black);
        sequence.AppendInterval(0.2f);
        sequence.AppendCallback(() => GetText((int)Texts.ScoreBonus).color = Color.black);

        return sequence;
    }

    private Tween ShowScore(int score)
    {
        TextMeshProUGUI scoreText = GetText((int)Texts.ScoreText);
        scoreText.color = Color.black;

        return DOVirtual.Int(0, score, 2f, value =>
        {
            scoreText.SetText($"Score : {value}");
        }).SetEase(Ease.InQuad)
        .OnPlay(() => Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString(), gameObject));
    }

    // 이전에 획득했던 별 개수만큼 불투명하게 표시
    private void SetPreStar(int preStarCount)
    {
        for (int i = 0; i < preStarCount; i++)
        {
            _stars[i].Activate();
            _stars[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }

    }

    private void SetReceiptText(int[] scoreList, int clearReward)
    {
        // 텍스트 값 설정
        var scoreTextList = new[] {
            (int)Texts.Star1ScoreText,
            (int)Texts.Star2ScoreText,
            (int)Texts.Star3ScoreText,
        };

        for (int i = 0; i < scoreList.Length && i < scoreTextList.Length; i++)
        {
            GetText(scoreTextList[i]).SetText($"{scoreList[i]}");
        }

        GetText((int)Texts.ClearRewardText).SetText($"x {clearReward}");

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

        // 3. 사라지기 (스케일 다운)
        //sequence.Append(transform.DOScale(Vector3.zero, _scaleDuration).SetEase(Ease.InQuad));
        sequence.Play();

    }
    private Tween ShowStar(int starCount)
    {
        // 1. 새로운 시퀀스를 만듭니다.
        Sequence sequence = DOTween.Sequence();
        float interval = 0.5f; // 별이 나타나는 시간 간격 (0.5초)

        for (int i = 0; i < starCount; i++)
        {
            int index = i; // 클로저 문제를 피하기 위해 인덱스를 복사합니다.

            // 2. 시퀀스에 '콜백'을 추가합니다. 이 콜백은 별 활성화와 사운드 재생을 담당합니다.
            sequence.AppendCallback(() =>
            {
                _stars[index].Activate();
                _stars[index].GetComponent<Image>().color = Color.white;
                Managers.Sound.PlaySFX(SoundType.MiniGameUnloadSFX, MiniGameUnloadSoundSFX.PlusScore.ToString(), gameObject);
            });

            // 3. 다음 별이 나타나기 전까지 대기하는 간격을 추가합니다.
            sequence.AppendInterval(interval);
        }

        // 4. 완성된 시퀀스를 반환합니다.
        return sequence;
    }

    private Tween ShowTotalGold(int totalGold)
    {
        TextMeshProUGUI totalText = GetText((int)Texts.TotalGoldText);

        Sequence totalGoldSequence = DOTween.Sequence();
        
        totalGoldSequence.Append(DOVirtual.Color(totalText.color, Color.black, 0.0f, value =>
        {
            totalText.color = value;
        }));

        totalGoldSequence.Append(DOVirtual.Int(0, totalGold, 1f, value =>
        {
            totalText.SetText(value.ToString());
        })).OnComplete(() =>
        {
            // 별을 얻었을 때만 돈을 얻기 때문에, 0 이상일 때 Clear 표시
            if (totalGold > 0)
                GetImage((int)Images.ClearIconImage).color = Color.white;   
        });

        return totalGoldSequence;
    }


    public override void ClosePopupUI()
    {
        base.ClosePopupUI();
    }

    public void OnDestroy()
    {
        OnClickConfirmButton();
    }
}
