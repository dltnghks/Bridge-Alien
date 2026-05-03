using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIPlayerTaskPopup : UIPopup
{
    private const string DefaultTaskId = "C01_T01";
    private const string TaskCountExcludedTaskId = "C03_T04";
    private const int MinTaskCount = 1;
    private const int MaxTaskCount = 99;
    private const float TaskCountRepeatStartDelay = 0.35f;
    private const float TaskCountRepeatInterval = 0.08f;
    private static string s_lastExecutedTaskId = string.Empty;
    private static string s_lastSelectedTaskId = string.Empty;
    private static int s_lastTaskCount = MinTaskCount;

    enum Texts
    {
        ExperienceValueText,
        GravityAdaptationValueText,
        StrengthValueText,
        LuckValueText,
        ThumbnailText,
        ConfirmButtonText,
        TaskNumberText,
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
        PlusButton,
        MinusButton,
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
    private int _taskCount = s_lastTaskCount;
    private Coroutine _taskCountRepeatCoroutine;

    public Action<bool> OnClickUpgrade;

    public TaskAnimator TaskAnimator { get; private set; }
    public int TaskCount => _taskCount;

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
        GetButton((int)Buttons.PlusButton).gameObject.BindEvent(OnPressPlusButton, Define.UIEvent.PointerDown);
        GetButton((int)Buttons.PlusButton).gameObject.BindEvent(StopTaskCountRepeat, Define.UIEvent.PointerUp);
        GetButton((int)Buttons.PlusButton).gameObject.BindEvent(StopTaskCountRepeat, Define.UIEvent.BeginDrag);
        GetButton((int)Buttons.PlusButton).gameObject.BindEvent(StopTaskCountRepeat, Define.UIEvent.EndDrag);
        GetButton((int)Buttons.MinusButton).gameObject.BindEvent(OnPressMinusButton, Define.UIEvent.PointerDown);
        GetButton((int)Buttons.MinusButton).gameObject.BindEvent(StopTaskCountRepeat, Define.UIEvent.PointerUp);
        GetButton((int)Buttons.MinusButton).gameObject.BindEvent(StopTaskCountRepeat, Define.UIEvent.BeginDrag);
        GetButton((int)Buttons.MinusButton).gameObject.BindEvent(StopTaskCountRepeat, Define.UIEvent.EndDrag);

        InitTabGroup();
        InitTaskGroup();

        SetTaskStatText();
        SetTaskCountText();
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

    private void OnDisable()
    {
        StopTaskCountRepeat();
    }

    private void OnPressPlusButton()
    {
        StartTaskCountRepeat(1);
    }

    private void OnPressMinusButton()
    {
        StartTaskCountRepeat(-1);
    }

    private void StartTaskCountRepeat(int delta)
    {
        if (IsTaskCountExcluded(_selectedTaskData))
        {
            return;
        }

        StopTaskCountRepeat();
        SetTaskCount(_taskCount + delta);
        _taskCountRepeatCoroutine = StartCoroutine(RepeatTaskCountChange(delta));
    }

    private IEnumerator RepeatTaskCountChange(int delta)
    {
        yield return new WaitForSeconds(TaskCountRepeatStartDelay);

        while (true)
        {
            SetTaskCount(_taskCount + delta);
            yield return new WaitForSeconds(TaskCountRepeatInterval);
        }
    }

    private void StopTaskCountRepeat()
    {
        if (_taskCountRepeatCoroutine == null)
        {
            return;
        }

        StopCoroutine(_taskCountRepeatCoroutine);
        _taskCountRepeatCoroutine = null;
    }

    private void OnClickConfirmButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());

        if (_currentTaskButton == null)
        {
            Logger.LogWarning("Nothing to click");
            return;
        }

        int effectiveTaskCount = GetEffectiveTaskCount(_selectedTaskData);
        int taskCost = GetTaskCost(_selectedTaskData);

        if (Managers.Player.GetGold() < taskCost)
        {
            Logger.LogWarning("You do not have enough gold to complete task!");
            return;
        }

        OnClickUpgrade?.Invoke(true);
        PlayerTaskExecutionData executionData = new PlayerTaskExecutionData(
            _selectedTaskData,
            _currentTaskTab.TaskType,
            effectiveTaskCount,
            taskCost);
        Managers.UI.RequestPopup<UITaskProgressPopup>(executionData);

        s_lastExecutedTaskId = _selectedTaskData.TaskID;

        ClosePopupUI();
    }

    private void SelectInitialTask()
    {
        string taskId = s_lastSelectedTaskId;
        if (string.IsNullOrEmpty(taskId))
        {
            taskId = s_lastExecutedTaskId;
        }

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
        s_lastSelectedTaskId = _selectedTaskData.TaskID;

        _currentTaskButton.Select();
        SetTaskStatTextImage();
        SetTaskCountText();
        SetTaskConfirmButton();
    }

    private void SetTaskConfirmButton()
    {
        if (_currentTaskButton == null)
        {
            return;
        }

        if (GetTaskCost(_selectedTaskData) > Managers.Player.GetGold())
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

    private void SetTaskCount(int taskCount)
    {
        if (IsTaskCountExcluded(_selectedTaskData))
        {
            SetTaskCountText();
            return;
        }

        int clampedTaskCount = Mathf.Clamp(taskCount, MinTaskCount, MaxTaskCount);
        if (_taskCount == clampedTaskCount)
        {
            return;
        }

        _taskCount = clampedTaskCount;
        s_lastTaskCount = _taskCount;
        SetTaskCountText();
        RefreshTaskButtonGolds();
        SetTaskConfirmButton();
    }

    private void SetTaskCountText()
    {
        int effectiveTaskCount = GetEffectiveTaskCount(_selectedTaskData);
        GetText((int)Texts.TaskNumberText).text = effectiveTaskCount.ToString();

        bool isTaskCountEnabled = !IsTaskCountExcluded(_selectedTaskData);
        GetButton((int)Buttons.PlusButton).interactable = isTaskCountEnabled;
        GetButton((int)Buttons.MinusButton).interactable = isTaskCountEnabled;
    }

    private void RefreshTaskButtonGolds()
    {
        _uiTaskGroup?.RefreshTaskButtonGolds();
    }

    public int GetTaskCost(PlayerTaskData taskData)
    {
        if (taskData == null)
        {
            return 0;
        }

        return taskData.RequirementGold * GetEffectiveTaskCount(taskData);
    }

    public int GetEffectiveTaskCount(PlayerTaskData taskData)
    {
        if (IsTaskCountExcluded(taskData))
        {
            return MinTaskCount;
        }

        return _taskCount;
    }

    private bool IsTaskCountExcluded(PlayerTaskData taskData)
    {
        return taskData != null && taskData.TaskID == TaskCountExcludedTaskId;
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

public class PlayerTaskExecutionData
{
    public PlayerTaskData TaskData { get; }
    public Define.TaskType TaskType { get; }
    public int TaskCount { get; }
    public int TaskCost { get; }
    public int ActualLuckDelta { get; private set; }
    public bool IsApplied { get; private set; }

    public PlayerTaskExecutionData(PlayerTaskData taskData, Define.TaskType taskType, int taskCount, int taskCost)
    {
        TaskData = taskData;
        TaskType = taskType;
        TaskCount = taskCount;
        TaskCost = taskCost;
    }

    public void Apply()
    {
        if (IsApplied || TaskData == null)
        {
            return;
        }

        for (int i = 0; i < TaskCount; i++)
        {
            ActualLuckDelta += Random.Range(TaskData.LuckMinValue, TaskData.LuckMaxValue);
        }

        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, TaskData.FatigueValue * TaskCount);
        Managers.Player.AddStats(Define.PlayerStatsType.Experience, TaskData.ExperienceValue * TaskCount);
        Managers.Player.AddStats(Define.PlayerStatsType.Strength, TaskData.StrengthValue * TaskCount);
        Managers.Player.AddStats(Define.PlayerStatsType.GravityAdaptation, TaskData.GravityAdaptationValue * TaskCount);
        Managers.Player.AddStats(Define.PlayerStatsType.Luck, ActualLuckDelta);

        Managers.Player.AddGold(-TaskCost, "task_execute", TaskData.TaskID);
        Managers.Analytics.TrackTaskExecute(
            TaskData.TaskID,
            TaskData.TaskName,
            TaskType.ToString(),
            TaskCost,
            TaskData.FatigueValue * TaskCount,
            TaskData.ExperienceValue * TaskCount,
            TaskData.StrengthValue * TaskCount,
            TaskData.GravityAdaptationValue * TaskCount,
            TaskData.LuckMinValue * TaskCount,
            TaskData.LuckMaxValue * TaskCount,
            ActualLuckDelta,
            Managers.Player.GetGold());
        Managers.Analytics.Flush();

        IsApplied = true;
    }
}
