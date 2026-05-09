using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDefaultStageInfo : UISubItem
{
    enum Texts
    {
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
        StageInfoClearIcon,
    }

    enum Objects
    {
        InfoStar1,
        InfoStar2,
        InfoStar3,
        StageStoryPlayButton,
    }

    private UIStageStoryPlayButton _storyPlayButton;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

        _storyPlayButton = GetObject((int)Objects.StageStoryPlayButton).gameObject.GetOrAddComponent<UIStageStoryPlayButton>();

        return true;
    }

    public void Apply(StageData stageData, Define.ChapterType stageType)
    {
        Init();
        if (stageData == null)
        {
            return;
        }

        SetText((int)Texts.StageNameText, stageData.StageName);
        SetText((int)Texts.StageDescriptionText, stageData.StageDescription);
        SetText((int)Texts.StageRewardText, $"x {stageData.ClearReward}");

        var thumbnails = stageData.GetPopupThumbnails();
        var previewImage = GetImage((int)Images.StagePreviewImage);
        if (previewImage != null)
        {
            previewImage.sprite = GetThumbnail(thumbnails, 0, stageData.StageImage);
        }

        SetStageScore(stageData.ClearScoreList);

        int starCount = Managers.Player.GetStageClearInfo(stageType);
        SetStarImage(starCount, 3);
        SetStoryPlayButton(stageType, stageData.ClearEventID);
    }

    private void SetStageScore(int[] clearScoreList)
    {
        if (clearScoreList == null || clearScoreList.Length < 3)
        {
            return;
        }

        SetText((int)Texts.Star1Score, clearScoreList[0].ToString());
        SetText((int)Texts.Star2Score, clearScoreList[1].ToString());
        SetText((int)Texts.Star3Score, clearScoreList[2].ToString());
    }

    private void SetStarImage(int starCount, int maxStarCount)
    {
        var clearIcon = GetImage((int)Images.StageInfoClearIcon);
        if (clearIcon != null)
        {
            clearIcon.color = Color.clear;
        }

        for (int i = 0; i < 3; i++)
        {
            var starObject = GetObject((int)Objects.InfoStar1 + i);
            if (starObject == null)
            {
                continue;
            }

            starObject.SetActive(i < maxStarCount);
            var button = starObject.GetComponent<UIActiveButton>();
            if (button == null)
            {
                continue;
            }

            if (i + 1 <= starCount)
            {
                button.Activate();
            }
            else
            {
                button.Deactivate();
            }
        }

        if (starCount > 0 && clearIcon != null)
        {
            clearIcon.color = Color.white;
        }
    }

    private void SetText(int index, string value)
    {
        TextMeshProUGUI text = GetText(index);
        if (text != null)
        {
            text.SetText(value);
        }
    }

    private void SetStoryPlayButton(Define.ChapterType stageType, Define.EventDataID clearEventID)
    {
        bool isStageCleared = Managers.Player.GetStageClearInfo(stageType) > 0;
        Logger.Log($"SetStoryPlayButton - stageType: {stageType}, IsStageCleared: {isStageCleared}");

        _storyPlayButton?.Apply(stageType, clearEventID, isStageCleared);
    }

    private static Sprite GetThumbnail(System.Collections.Generic.List<Sprite> thumbnails, int index, Sprite fallback)
    {
        if (thumbnails != null && thumbnails.Count > index && thumbnails[index] != null)
        {
            return thumbnails[index];
        }

        return fallback;
    }
}
