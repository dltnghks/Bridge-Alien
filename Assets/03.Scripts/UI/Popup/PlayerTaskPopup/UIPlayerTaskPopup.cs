using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIPlayerTaskPopup : UIPopup
{
    private const string DefaultTaskId = "C01_T01";
    private static string s_lastExecutedTaskId = string.Empty;

    enum Texts
    {
        ExperienceValueText,
        GravityAdaptationValueText,
        StrengthValueText,
        LuckValueText,
        ThumbnailText,
        ConfirmButtonText,
    }

    enum Images
    {
        ExperienceValueTextIncreaseImage,
        GravityAdaptationValueTextIncreaseImage,
        StrengthValueTextIncreaseImage,
        LuckValueTextIncreaseImage,
        ExperienceValueTextDecreaseImage,
        GravityAdaptationValueTextDecreaseImage,
        StrengthValueTextDecreaseImage,
        LuckValueTextDecreaseImage,
    }

    enum Buttons
    {
        SelfDevelopmentButton,
        EntertainmentButton,
        InvestmentButton,
        ConfirmButton,
    }

    enum Objects
    {
        UITaskGroup,
        UITaskAnimPortrait,
    }

    private UITaskTabButton _currentTaskTab;
    private UITaskButton _currentTaskButton;
    private UITaskGroup _uiTaskGroup;
    private UIActiveButton _uiConfirmButton;
    private ScrollRect _scrollRect;
    private PlayerTaskData _selectedTaskData;

    public Action<bool> OnClickUpgrade;

    public TaskAnimator TaskAnimator { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindButton(typeof(Buttons));
        BindImage(typeof(Images));
        BindObject(typeof(Objects));
        BindText(typeof(Texts));

        TaskAnimator = GetObject((int)Objects.UITaskAnimPortrait).GetComponent<TaskAnimator>();

        _uiConfirmButton = GetButton((int)Buttons.ConfirmButton).gameObject.GetOrAddComponent<UIActiveButton>();
        _uiConfirmButton.Init();
        _uiConfirmButton.gameObject.BindEvent(OnClickConfirmButton);

        InitTabGroup();
        InitTaskGroup();

        SetTaskStatText();
        SelectInitialTask();

        return true;
    }

    private void InitTabGroup()
    {
        GetButton((int)Buttons.SelfDevelopmentButton).GetOrAddComponent<UITaskTabButton>().Init(this, Define.TaskType.SelfDevelopment);
        GetButton((int)Buttons.EntertainmentButton).GetOrAddComponent<UITaskTabButton>().Init(this, Define.TaskType.Entertainment);
        GetButton((int)Buttons.InvestmentButton).GetOrAddComponent<UITaskTabButton>().Init(this, Define.TaskType.Fortune);
    }

    private void InitTaskGroup()
    {
        _uiTaskGroup = GetObject((int)Objects.UITaskGroup).GetOrAddComponent<UITaskGroup>();
        _uiTaskGroup.Init(this);
        _scrollRect = _uiTaskGroup.GetOrAddComponent<ScrollRect>();
    }

    private void OnClickConfirmButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());

        if (_currentTaskButton == null)
        {
            Logger.LogWarning("Nothing to click");
            return;
        }

        if (Managers.Player.GetGold() < _selectedTaskData.RequirementGold)
        {
            Logger.LogWarning("You do not have enough gold to complete task!");
            return;
        }

        int actualLuckDelta = Random.Range(_selectedTaskData.LuckMinValue, _selectedTaskData.LuckMaxValue);

        OnClickUpgrade?.Invoke(true);
        Managers.UI.RequestPopup<UITaskProgressPopup>(_selectedTaskData);

        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, _selectedTaskData.FatigueValue);
        Managers.Player.AddStats(Define.PlayerStatsType.Experience, _selectedTaskData.ExperienceValue);
        Managers.Player.AddStats(Define.PlayerStatsType.Strength, _selectedTaskData.StrengthValue);
        Managers.Player.AddStats(Define.PlayerStatsType.GravityAdaptation, _selectedTaskData.GravityAdaptationValue);
        Managers.Player.AddStats(Define.PlayerStatsType.Luck, actualLuckDelta);

        Managers.Player.AddGold(-_selectedTaskData.RequirementGold, "task_execute", _selectedTaskData.TaskID);
        s_lastExecutedTaskId = _selectedTaskData.TaskID;
        Managers.Analytics.TrackTaskExecute(
            _selectedTaskData.TaskID,
            _selectedTaskData.TaskName,
            _currentTaskTab.TaskType.ToString(),
            _selectedTaskData.RequirementGold,
            _selectedTaskData.FatigueValue,
            _selectedTaskData.ExperienceValue,
            _selectedTaskData.StrengthValue,
            _selectedTaskData.GravityAdaptationValue,
            _selectedTaskData.LuckMinValue,
            _selectedTaskData.LuckMaxValue,
            actualLuckDelta,
            Managers.Player.GetGold());
        Managers.Analytics.Flush();

        ClosePopupUI();
    }

    private void SelectInitialTask()
    {
        string taskId = s_lastExecutedTaskId;
        if (string.IsNullOrEmpty(taskId))
        {
            taskId = DefaultTaskId;
        }

        if (!Managers.Data.PlayerTaskData.TryGetTaskType(taskId, out Define.TaskType taskType))
        {
            taskId = DefaultTaskId;
            taskType = Define.TaskType.SelfDevelopment;
        }

        UITaskTabButton taskTabButton = GetTaskTabButton(taskType);
        if (taskTabButton == null)
        {
            taskTabButton = GetButton((int)Buttons.SelfDevelopmentButton).GetComponent<UITaskTabButton>();
            taskId = DefaultTaskId;
        }

        SelectTabButton(taskTabButton);
        SelectTaskButtonById(taskId);
    }

    public void SelectTabButton(UITaskTabButton taskTabButton)
    {
        if (_currentTaskTab != null)
        {
            _currentTaskTab.Deselect();
        }

        _currentTaskTab = taskTabButton;
        _currentTaskTab.Select();
        SetTaskGroup();
        _scrollRect.verticalNormalizedPosition = 1.0f;
    }

    private void SetTaskGroup()
    {
        Logger.Log($"{_currentTaskTab.TaskType} task group set");

        if (_uiTaskGroup == null)
        {
            return;
        }

        _uiTaskGroup.Setup(_currentTaskTab.TaskType);
        _currentTaskButton?.Deselect();

        foreach (var taskButton in _uiTaskGroup.TaskButtons)
        {
            if (taskButton.PlayerTaskData == _selectedTaskData)
            {
                SelectTaskButton(taskButton);
                return;
            }
        }

        if (_uiTaskGroup.TaskButtons.Count > 0)
        {
            SelectTaskButton(_uiTaskGroup.TaskButtons[0]);
        }
    }

    private UITaskTabButton GetTaskTabButton(Define.TaskType taskType)
    {
        return taskType switch
        {
            Define.TaskType.SelfDevelopment => GetButton((int)Buttons.SelfDevelopmentButton).GetComponent<UITaskTabButton>(),
            Define.TaskType.Entertainment => GetButton((int)Buttons.EntertainmentButton).GetComponent<UITaskTabButton>(),
            Define.TaskType.Fortune => GetButton((int)Buttons.InvestmentButton).GetComponent<UITaskTabButton>(),
            _ => null,
        };
    }

    private void SelectTaskButtonById(string taskId)
    {
        if (string.IsNullOrEmpty(taskId) || _uiTaskGroup == null)
        {
            return;
        }

        foreach (var taskButton in _uiTaskGroup.TaskButtons)
        {
            if (taskButton.PlayerTaskData != null && taskButton.PlayerTaskData.TaskID == taskId)
            {
                SelectTaskButton(taskButton);
                return;
            }
        }
    }

    public void SelectTaskButton(UITaskButton taskButton)
    {
        if (_currentTaskButton != null)
        {
            _currentTaskButton.Deselect();
        }

        _currentTaskButton = taskButton;
        _selectedTaskData = _currentTaskButton.PlayerTaskData;

        _currentTaskButton.Select();
        SetTaskStatTextImage();
        SetTaskConfirmButton();
    }

    private void SetTaskConfirmButton()
    {
        if (_currentTaskButton == null)
        {
            return;
        }

        if (_selectedTaskData.RequirementGold > Managers.Player.GetGold())
        {
            _uiConfirmButton?.Deactivate();
            GetText((int)Texts.ConfirmButtonText).text = "\uC18C\uC9C0\uAE08 \uBD80\uC871";
        }
        else
        {
            _uiConfirmButton?.Activate();
            GetText((int)Texts.ConfirmButtonText).text = "\uC218\uD589\uD558\uAE30";
        }
    }

    private void SetTaskStatText()
    {
        PlayerData playerData = Managers.Player.PlayerData;
        GetText((int)Texts.ExperienceValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Experience]} / 100";
        GetText((int)Texts.GravityAdaptationValueText).text = $"{playerData.Stats[Define.PlayerStatsType.GravityAdaptation]} / 100";
        GetText((int)Texts.StrengthValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Strength]} / 100";
        GetText((int)Texts.LuckValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Luck]} / 100";
        SetTaskStatImages(false);
    }

    private void SetTaskStatTextImage()
    {
        TaskAnimator.TriggerTask(_selectedTaskData.TaskID);

        SetTaskStatImages(false);

        if (_selectedTaskData.ExperienceValue > 0)
        {
            GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        else if (_selectedTaskData.ExperienceValue < 0)
        {
            GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }

        if (_selectedTaskData.StrengthValue > 0)
        {
            GetImage((int)Images.StrengthValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        else if (_selectedTaskData.StrengthValue < 0)
        {
            GetImage((int)Images.StrengthValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }

        if (_selectedTaskData.GravityAdaptationValue > 0)
        {
            GetImage((int)Images.GravityAdaptationValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        else if (_selectedTaskData.GravityAdaptationValue < 0)
        {
            GetImage((int)Images.GravityAdaptationValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }

        if (_selectedTaskData.LuckMinValue != 0)
        {
            GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }

        GetText((int)Texts.ThumbnailText).text = _selectedTaskData.ThumbnailText;
    }

    private void SetTaskStatImages(bool active)
    {
        float value = active ? 1.0f : 0.0f;
        GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.StrengthValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.GravityAdaptationValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);

        GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.StrengthValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.GravityAdaptationValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.LuckValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
    }
}
