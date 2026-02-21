using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStagePopup : UIPopup
{
    private interface IStagePopupTemplateRenderer
    {
        StagePopupTemplateType TemplateType { get; }
        void Render(UIStagePopup popup, StageData stageData, List<Sprite> thumbnails);
    }

    private class DefaultStagePopupTemplateRenderer : IStagePopupTemplateRenderer
    {
        public StagePopupTemplateType TemplateType => StagePopupTemplateType.Default;

        public void Render(UIStagePopup popup, StageData stageData, List<Sprite> thumbnails)
        {
            popup.SetTemplateRoots(isDefaultActive: true, isEndingActive: false);
            popup.SetPrimaryThumbnail(GetThumbnail(thumbnails, 0, stageData.StageImage));
            popup.SetSecondaryThumbnail(null, isVisible: false);
        }
    }

    private class EndingStagePopupTemplateRenderer : IStagePopupTemplateRenderer
    {
        public StagePopupTemplateType TemplateType => StagePopupTemplateType.Ending;

        public void Render(UIStagePopup popup, StageData stageData, List<Sprite> thumbnails)
        {
            popup.SetTemplateRoots(isDefaultActive: false, isEndingActive: true);
            int unlockedIndex = popup.GetUnlockedEndingThumbnailIndex();

            Sprite firstEndingThumbnail = GetThumbnail(thumbnails, 0, null);
            Sprite secondEndingThumbnail = GetThumbnail(thumbnails, 1, null);
            Sprite firstLockedThumbnail = stageData.GetEndingLockedThumbnail(0);
            Sprite secondLockedThumbnail = stageData.GetEndingLockedThumbnail(1);

            Sprite firstPreview = unlockedIndex == 0 && firstEndingThumbnail != null ? firstEndingThumbnail : firstLockedThumbnail;
            Sprite secondPreview = unlockedIndex == 1 && secondEndingThumbnail != null ? secondEndingThumbnail : secondLockedThumbnail;

            popup.SetPrimaryThumbnail(firstPreview);
            popup.SetSecondaryThumbnail(secondPreview, isVisible: secondPreview != null);
        }
    }

    enum Buttons
    {
        StageStartButton,
    }

    enum Texts
    {
        //StageTitleText,
        StageNameText,
        StageDescriptionText,
        StageRewardText,
        Star1Score,
        Star2Score,
        Star3Score,

    }

    enum Images
    {
        StagePreviewImage,
        StagePreviewImageSecondary,
        StageInfoClearIcon,
    }

    enum Objects
    {
        UIStageButtonGroup,
        DefaultTemplateRoot,
        EndingTemplateRoot,
        InfoStar1,
        InfoStar2,
        InfoStar3,
    }

    private UIStageButtonGroup _stageButtonGroup;
    private readonly Dictionary<StagePopupTemplateType, IStagePopupTemplateRenderer> _templateRenderers = new Dictionary<StagePopupTemplateType, IStagePopupTemplateRenderer>();

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

        if (_templateRenderers.Count == 0)
        {
            var defaultRenderer = new DefaultStagePopupTemplateRenderer();
            var endingRenderer = new EndingStagePopupTemplateRenderer();
            _templateRenderers[defaultRenderer.TemplateType] = defaultRenderer;
            _templateRenderers[endingRenderer.TemplateType] = endingRenderer;
        }

        var stageButtonGroupObject = GetObject((int)Objects.UIStageButtonGroup);
        if (stageButtonGroupObject != null)
        {
            _stageButtonGroup = stageButtonGroupObject.GetOrAddComponent<UIStageButtonGroup>();
        }

        var stageStartButton = GetButton((int)Buttons.StageStartButton);
        if (stageStartButton != null)
        {
            stageStartButton.gameObject.BindEvent(OnClickStageStartButton);
        }

        // 스테이지 매니저의 정보가 변경되는 경우 UI에 표시해주기
        Managers.Stage.OnChangeStage += SetStageInfo;
        SetStageInfo(Managers.Stage.GetCurrentStageData());

        return true;
    }

    public void InitStageButtonGroup()
    {
        if (_stageButtonGroup != null)
        {
            _stageButtonGroup.InitStageButtonGroup();
        }
    }

    private void OnClickStageStartButton()
    {
        Managers.Stage.StartStage();
    }

    public void SetStageInfo(StageData stageData)
    {
        if (stageData == null)
        {
            return;
        }

        var stageType = Managers.Stage.CurrentStageType;

        // 스테이지 표시
        string stageText = Managers.Stage.ToStageString(stageType);
        //GetText((int)Texts.StageTitleText).SetText($"Stage {stageText}");

        // 스테이지 이름 표시
        var stageNameText = GetText((int)Texts.StageNameText);
        if (stageNameText != null)
        {
            stageNameText.SetText(stageData.StageName);
        }

        // 스테이지 설명 표시
        var stageDescriptionText = GetText((int)Texts.StageDescriptionText);
        if (stageDescriptionText != null)
        {
            stageDescriptionText.SetText(stageData.StageDescription);
        }

        var thumbnails = stageData.GetPopupThumbnails();
        ApplyTemplate(stageData, thumbnails);

        // 보상 표시
        SetReward(stageData.ClearReward);

        // 스테이지 별 점수 표시
        SetStageScore(stageData.ClearScoreList);

        // 클리어 별 개수 표시
        Managers.Player.PlayerData.ClearedStages.TryGetValue(stageType, out int starCount);
        SetStageStarImage(starCount);
    }

    private void ApplyTemplate(StageData stageData, List<Sprite> thumbnails)
    {
        if (_templateRenderers.TryGetValue(stageData.PopupTemplateType, out var renderer) == false)
        {
            renderer = _templateRenderers[StagePopupTemplateType.Default];
        }

        renderer.Render(this, stageData, thumbnails);
    }

    private void SetPrimaryThumbnail(Sprite stageImage)
    {
        var previewImage = GetImage((int)Images.StagePreviewImage);
        if (previewImage != null)
        {
            previewImage.sprite = stageImage;
        }
    }

    private void SetSecondaryThumbnail(Sprite stageImage, bool isVisible)
    {
        var secondaryPreviewImage = GetImage((int)Images.StagePreviewImageSecondary);
        if (secondaryPreviewImage == null)
        {
            return;
        }

        secondaryPreviewImage.sprite = stageImage;
        secondaryPreviewImage.gameObject.SetActive(isVisible && stageImage != null);
    }

    private void SetTemplateRoots(bool isDefaultActive, bool isEndingActive)
    {
        SetObjectActive((int)Objects.DefaultTemplateRoot, isDefaultActive);
        SetObjectActive((int)Objects.EndingTemplateRoot, isEndingActive);
    }

    private void SetObjectActive(int objectIndex, bool isActive)
    {
        var target = GetObject(objectIndex);
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }

    private void SetReward(int reward)
    {
        var rewardText = GetText((int)Texts.StageRewardText);
        if (rewardText != null)
        {
            rewardText.SetText($"x {reward}");
        }
    }

    // 스테이지 별 점수 표시
    private void SetStageScore(int[] clearScoreList)
    {
        if (clearScoreList == null)
        {
            return;
        }

        if (clearScoreList.Length >= 3)
        {
            var star1ScoreText = GetText((int)Texts.Star1Score);
            var star2ScoreText = GetText((int)Texts.Star2Score);
            var star3ScoreText = GetText((int)Texts.Star3Score);

            if (star1ScoreText != null) star1ScoreText.SetText(clearScoreList[0].ToString());
            if (star2ScoreText != null) star2ScoreText.SetText(clearScoreList[1].ToString());
            if (star3ScoreText != null) star3ScoreText.SetText(clearScoreList[2].ToString());
        }
    }

    private void SetStageStarImage(int starCount)
    {
        var stageInfoClearIcon = GetImage((int)Images.StageInfoClearIcon);
        if (stageInfoClearIcon != null)
        {
            stageInfoClearIcon.color = Color.clear;
        }

        for (int i = 1; i <= 3; i++)
        {
            var starObject = GetObject((int)Objects.InfoStar1 + i - 1);
            var activeButton = starObject != null ? starObject.GetComponent<UIActiveButton>() : null;
            if (activeButton != null)
            {
                activeButton.Deactivate();
            }
        }

        if (starCount >= 1)
        {
            var star1 = GetObject((int)Objects.InfoStar1);
            var activeButton = star1 != null ? star1.GetComponent<UIActiveButton>() : null;
            if (activeButton != null)
            {
                activeButton.Activate();
            }
        }

        if (starCount >= 2)
        {
            var star2 = GetObject((int)Objects.InfoStar2);
            var activeButton = star2 != null ? star2.GetComponent<UIActiveButton>() : null;
            if (activeButton != null)
            {
                activeButton.Activate();
            }
        }

        if (starCount >= 3)
        {
            var star3 = GetObject((int)Objects.InfoStar3);
            var activeButton = star3 != null ? star3.GetComponent<UIActiveButton>() : null;
            if (activeButton != null)
            {
                activeButton.Activate();
            }
        }

        if (starCount > 0 && stageInfoClearIcon != null)
        {
            stageInfoClearIcon.color = Color.white;
        }
    }

    private void OnDestroy()
    {
        Managers.Stage.OnChangeStage -= SetStageInfo;
    }

    private static Sprite GetThumbnail(List<Sprite> thumbnails, int index, Sprite fallback)
    {
        if (thumbnails != null && thumbnails.Count > index && thumbnails[index] != null)
        {
            return thumbnails[index];
        }

        return fallback;
    }

    private int GetUnlockedEndingThumbnailIndex()
    {
        return Managers.Player.GetEndingThumbnailProgress(Managers.Stage.CurrentStageType);
    }
}
