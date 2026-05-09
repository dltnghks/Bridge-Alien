using UnityEngine.UI;

public class UIStageStoryPlayButton : UISubItem
{
    private const string StageStoryPlayButtonName = "StageStoryPlayButton";

    private Button _storyPlayButton;
    private Define.ChapterType _stageType;

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

    public void Apply(Define.ChapterType stageType, bool isVisible)
    {
        Init();

        _stageType = stageType;
        if (_storyPlayButton != null)
        {
            _storyPlayButton.gameObject.SetActive(isVisible);
        }
    }

    private void OnClickStoryPlayButton()
    {
        Logger.Log($"Story Play Button Clicked: {_stageType}");
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
    }

    private Button FindStoryPlayButton()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name == StageStoryPlayButtonName)
            {
                return button;
            }
        }

        Logger.Log($"Failed to bind({StageStoryPlayButtonName})");
        return null;
    }
}
