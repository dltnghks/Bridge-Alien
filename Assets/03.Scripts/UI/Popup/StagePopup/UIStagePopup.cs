using UnityEngine;
using UnityEngine.UI;

public class UIStagePopup : UIPopup
{
    private const string StageIntroPlayButtonName = "StageIntroPlayButton";

    enum Buttons
    {
        StageStartButton,
    }

    enum Objects
    {
        UIStageButtonGroup,
        DefaultTemplateRoot,
        EndingTemplateRoot,
    }

    private UIStageButtonGroup _stageButtonGroup;
    private UIDefaultStageInfo _defaultStageInfo;
    private UIEndingStageInfo _endingStageInfo;
    private Button _stageIntroPlayButton;
    private Define.EventDataID _stageIntroEventID;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindObject(typeof(Objects));

        var stageButtonGroupObject = GetObject((int)Objects.UIStageButtonGroup);
        if (stageButtonGroupObject != null)
        {
            _stageButtonGroup = stageButtonGroupObject.GetOrAddComponent<UIStageButtonGroup>();
        }

        var defaultTemplateRoot = GetObject((int)Objects.DefaultTemplateRoot);
        if (defaultTemplateRoot != null)
        {
            _defaultStageInfo = defaultTemplateRoot.GetOrAddComponent<UIDefaultStageInfo>();
        }
        else
        {
            _defaultStageInfo = gameObject.GetOrAddComponent<UIDefaultStageInfo>();
        }

        var endingTemplateRoot = GetObject((int)Objects.EndingTemplateRoot);
        if (endingTemplateRoot != null)
        {
            _endingStageInfo = endingTemplateRoot.GetOrAddComponent<UIEndingStageInfo>();
        }
        else
        {
            _endingStageInfo = gameObject.GetOrAddComponent<UIEndingStageInfo>();
        }

        var stageStartButton = GetButton((int)Buttons.StageStartButton);
        if (stageStartButton != null)
        {
            stageStartButton.gameObject.BindEvent(OnClickStageStartButton);
        }

        _stageIntroPlayButton = FindButton(StageIntroPlayButtonName);
        if (_stageIntroPlayButton != null)
        {
            _stageIntroPlayButton.gameObject.BindEvent(OnClickStageIntroPlayButton);
        }

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

    private void OnClickStageIntroPlayButton()
    {
        Logger.Log($"Stage Intro Play Button Clicked: {_stageIntroEventID}");
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        Managers.Stage.PlayStageStory(_stageIntroEventID);
    }

    public void SetStageInfo(StageData stageData)
    {
        if (stageData == null)
        {
            return;
        }

        var stageType = Managers.Stage.CurrentStageType;
        bool isEndingTemplate = stageData.PopupTemplateType == StagePopupTemplateType.Ending;

        SetStageIntroPlayButton(stageData, stageType);
        SetTemplateRootActive((int)Objects.DefaultTemplateRoot, !isEndingTemplate);
        SetTemplateRootActive((int)Objects.EndingTemplateRoot, isEndingTemplate);

        if (isEndingTemplate)
        {
            if (_endingStageInfo != null)
            {
                _endingStageInfo.Apply(stageData, stageType);
            }
        }
        else
        {
            if (_defaultStageInfo != null)
            {
                _defaultStageInfo.Apply(stageData, stageType);
            }
        }
    }

    private void SetTemplateRootActive(int objectIndex, bool isActive)
    {
        var rootObject = GetObject(objectIndex);
        if (rootObject != null)
        {
            rootObject.SetActive(isActive);
        }
    }

    private void SetStageIntroPlayButton(StageData stageData, Define.ChapterType stageType)
    {
        _stageIntroEventID = stageData.EventID;
        if (_stageIntroPlayButton == null)
        {
            return;
        }

        bool isVisible = Managers.Player.GetStageProgressedStatus(stageType) &&
            _stageIntroEventID != Define.EventDataID.Unknown;
        _stageIntroPlayButton.gameObject.SetActive(isVisible);
    }

    private Button FindButton(string buttonName)
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name == buttonName)
            {
                return button;
            }
        }

        Logger.Log($"Failed to bind({buttonName})");
        return null;
    }

    private void OnDestroy()
    {
        Managers.Stage.OnChangeStage -= SetStageInfo;
    }
}
