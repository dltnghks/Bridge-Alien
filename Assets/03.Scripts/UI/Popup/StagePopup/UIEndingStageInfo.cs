using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEndingStageInfo : UISubItem
{
    enum Texts
    {
        StageNameText,
        StageGoalText,
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
        StageThumnailIcon,
        StageInfoClearIcon,
    }

    enum Objects
    {
        StageGoalStarGroup,
        StageReward,
        InfoStar1,
        InfoStar2,
        InfoStar3,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));

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
        SetText((int)Texts.StageGoalText, stageData.StageDescription);
        SetText((int)Texts.StageDescriptionText, stageData.StageDescription);
        SetText((int)Texts.StageRewardText, "???");
        SetStageScore(stageData.ClearScoreList);

        var thumbnails = stageData.GetPopupThumbnails();
        Sprite unlocked0 = GetThumbnail(thumbnails, 0, null);
        Sprite unlocked1 = GetThumbnail(thumbnails, 1, null);
        Sprite locked0 = stageData.GetEndingLockedThumbnail(0);
        Sprite locked1 = stageData.GetEndingLockedThumbnail(1);

        bool isUnlocked0 = Managers.Player.IsEndingThumbnailUnlocked(stageType, 0);
        bool isUnlocked1 = Managers.Player.IsEndingThumbnailUnlocked(stageType, 1);

        SetPrimaryThumbnail(isUnlocked0 && unlocked0 != null ? unlocked0 : locked0);

        Sprite secondary = isUnlocked1 && unlocked1 != null ? unlocked1 : locked1;
        if (secondary == null)
        {
            secondary = locked0;
        }
        SetSecondaryThumbnail(secondary, true);

        int starCount = Managers.Player.GetStageClearInfo(stageType);
        SetStarImage(starCount, 3);

        SetObjectActive((int)Objects.StageGoalStarGroup, true);
        SetObjectActive((int)Objects.StageReward, false);



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

    private void SetPrimaryThumbnail(Sprite sprite)
    {
        var image = GetImage((int)Images.StagePreviewImage);
        if (image != null)
        {
            image.sprite = sprite;
        }
    }

    private void SetSecondaryThumbnail(Sprite sprite, bool isVisible)
    {
        var image = GetImage((int)Images.StagePreviewImageSecondary);
        if (image == null)
        {
            image = GetImage((int)Images.StageThumnailIcon);
        }

        if (image == null)
        {
            return;
        }

        image.sprite = sprite;
        image.gameObject.SetActive(isVisible);
    }

    private void SetStarImage(int starCount, int maxStarCount)
    {
        var clearIcon = GetImage((int)Images.StageInfoClearIcon);
        if (clearIcon != null)
        {
            clearIcon.gameObject.SetActive(true);
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

    private static Sprite GetThumbnail(System.Collections.Generic.List<Sprite> thumbnails, int index, Sprite fallback)
    {
        if (thumbnails != null && thumbnails.Count > index && thumbnails[index] != null)
        {
            return thumbnails[index];
        }

        return fallback;
    }

    private void SetObjectActive(int objectIndex, bool isActive)
    {
        var target = GetObject(objectIndex);
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }
}



