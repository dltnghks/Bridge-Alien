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
    }

    enum Objects
    {
        UIStageButtonGroup,
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
        BindObject(typeof(Objects));

        _stageButtonGroup = GetObject((int)Objects.UIStageButtonGroup).GetOrAddComponent<UIStageButtonGroup>();

        GetButton((int)Buttons.StageStartButton).gameObject.BindEvent(OnClickStageStartButton);

        // 스테이지 매니저의 정보가 변경되는 경우 UI에 표시해주기
        Managers.Stage.OnChangeStage += SetStageInfo;

        InitStageButtonGroup();

        return true;
    }

    private void InitStageButtonGroup()
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

        // 보상 표시
        string reward = stageData.ClearReward.ToString();
        GetText((int)Texts.StageRewardText).SetText(reward);
    }

}
