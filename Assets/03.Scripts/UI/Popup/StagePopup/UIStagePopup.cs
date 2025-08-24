using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStagePopup : UIPopup
{
    enum Buttons
    {
        StageStartButton,
    }

    enum Texts
    {
        StageNameText,
        StageRewardText,
        Star1Score,
        Star2Score,
        Star3Score,

    }

    enum Images
    {
        StagePreviewImage,
    }

    enum Objects
    {
        UIStageButtonGroup,
        InfoStar1,
        InfoStar2,
        InfoStar3,
    }

    private UIStageButtonGroup _stageButtonGroup;

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

        _stageButtonGroup = GetObject((int)Objects.UIStageButtonGroup).GetOrAddComponent<UIStageButtonGroup>();

        GetButton((int)Buttons.StageStartButton).gameObject.BindEvent(OnClickStageStartButton);

        // 스테이지 매니저의 정보가 변경되는 경우 UI에 표시해주기
        Managers.Stage.OnChangeStage += SetStageInfo;
        SetStageInfo(Managers.Stage.GetCurrentStageData());

        return true;
    }

    public void InitStageButtonGroup()
    {
        _stageButtonGroup.InitStageButtonGroup();
    }

    private void OnClickStageStartButton()
    {
        Managers.Scene.ChangeScene(Define.Scene.MiniGameUnload);
    }

    public void SetStageInfo(StageData stageData)
    {
        // 스테이지 이름 표시
        GetText((int)Texts.StageNameText).SetText(stageData.StageName);

        // 스테이지 이미지 표시
        SetStagePreviewImage(stageData.StageImage);

        // 보상 표시
        SetReward(stageData.ClearReward);

        // 스테이지 별 점수 표시
        SetStageScore(stageData.ClearScoreList);

        // 클리어 별 개수 표시
        Managers.Player.PlayerData.ClearedStages.TryGetValue(Managers.Stage.CurrentStageType, out int starCount);
        SetStageStarImage(starCount);
    }

    private void SetStagePreviewImage(Sprite stageImage)
    {
        GetImage((int)Images.StagePreviewImage).sprite = stageImage;
    }

    private void SetReward(int reward)
    {
        GetText((int)Texts.StageRewardText).SetText(reward.ToString());
    }

    // 스테이지 별 점수 표시
    private void SetStageScore(int[] clearScoreList)
    {
        if (clearScoreList.Length >= 3)
        {
            GetText((int)Texts.Star1Score).SetText(clearScoreList[0].ToString());
            GetText((int)Texts.Star2Score).SetText(clearScoreList[1].ToString());
            GetText((int)Texts.Star3Score).SetText(clearScoreList[2].ToString());
        }
    }

    private void SetStageStarImage(int starCount)
    {
        for (int i = 1; i <= 3; i++)
        {
            GetObject((int)Objects.InfoStar1 + i - 1).GetComponent<UIActiveButton>().Deactivate();
        }

        if (starCount >= 1)
        {
            GetObject((int)Objects.InfoStar1).GetComponent<UIActiveButton>().Activate();
        }

        if (starCount >= 2)
        {
            GetObject((int)Objects.InfoStar2).gameObject.GetComponent<UIActiveButton>().Activate();
        }

        if (starCount >= 3)
        {
            GetObject((int)Objects.InfoStar3).gameObject.GetComponent<UIActiveButton>().Activate();
        }
    }

    private void OnDestroy()
    {
        Managers.Stage.OnChangeStage -= SetStageInfo;
    }

}
