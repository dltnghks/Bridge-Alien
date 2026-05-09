using UnityEngine.UI;

public class UIStageStoryPlayButton : UISubItem
{
    private Button _storyPlayButton;
    private Define.ChapterType _stageType;
    private Define.EventDataID _storyEventID;

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _storyPlayButton = FindStoryPlayButton();
        if (_storyPlayButton != null)
        {
            _storyPlayButton.gameObject.BindEvent(OnClickStoryPlayButton);
        }

        return true;
    }

    public void Apply(Define.ChapterType stageType, Define.EventDataID storyEventID, bool isVisible)
    {
        Init();

        _stageType = stageType;
        _storyEventID = storyEventID;
        if (_storyPlayButton != null)
        {
            _storyPlayButton.gameObject.SetActive(isVisible && _storyEventID != Define.EventDataID.Unknown);
        }
    }

    private void OnClickStoryPlayButton()
    {
        Logger.Log($"Story Play Button Clicked: {_stageType}, EventID: {_storyEventID}");
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());

        Managers.Stage.PlayStageStory(_storyEventID);
    }

    private Button FindStoryPlayButton()
    {
        Button selfButton = GetComponent<Button>();
        if (selfButton != null)
        {
            return selfButton;
        }

        Button[] buttons = GetComponentsInChildren<Button>(true);
        if (buttons.Length > 0)
        {
            return buttons[0];
        }

        Logger.Log($"Failed to bind({nameof(UIStageStoryPlayButton)})");
        return null;
    }
}
