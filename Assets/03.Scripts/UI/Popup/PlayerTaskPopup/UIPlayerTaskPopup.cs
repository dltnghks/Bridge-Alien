using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIPlayerTaskPopup : UIPopup
{
    enum Texts
    {
        ExperienceValueText,
        GravityAdaptationValueText,
        IntelligenceValueText,
        LuckValueText,
    }

    enum Images
    {
        BlurBackground,
        ExperienceValueTextIncreaseImage,
        GravityAdaptationValueTextIncreaseImage,
        IntelligenceValueTextIncreaseImage,
        LuckValueTextIncreaseImage,
        ExperienceValueTextDecreaseImage,
        GravityAdaptationValueTextDecreaseImage,
        IntelligenceValueTextDecreaseImage,
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

    private Define.TaskType _currentTaskType = Define.TaskType.Unknown;
    private UITaskTabButton _currentTaskTab = null;
    private UITaskButton _currentTaskButton = null;
    private UITaskGroup _uiTaskGroup = null;
    private ScrollRect _scrollRect = null;
    
    public TaskAnimator TaskAnimator
    {
        get;
        private set;
    }

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
        
        GetImage((int)Images.BlurBackground).gameObject.BindEvent(OnClickBlurBackground);

        InitTabGroup();
        InitTaskGroup();
        
        GetButton((int)Buttons.ConfirmButton).gameObject.BindEvent(OnClickConfirmButton);
        
        SetTaskStatText();
        
        // 초기화가 끝난 후, 첫 번째 탭 선택
        SelectTabButton(GetButton((int)Buttons.SelfDevelopmentButton).GetComponent<UITaskTabButton>());
        
        return true;
    }

    private void InitTabGroup()
    {
        // 버튼 3개 세팅
        GetButton((int)Buttons.SelfDevelopmentButton).GetOrAddComponent<UITaskTabButton>().Init(this, Define.TaskType.SelfDevelopment);
        GetButton((int)Buttons.EntertainmentButton).GetOrAddComponent<UITaskTabButton>().Init(this, Define.TaskType.Entertainment);
        GetButton((int)Buttons.InvestmentButton).GetOrAddComponent<UITaskTabButton>().Init(this, Define.TaskType.Fortune);
    }
        

    private void InitTaskGroup()
    {
        _uiTaskGroup = GetObject((int)Objects.UITaskGroup).GetOrAddComponent<UITaskGroup>();
        _uiTaskGroup.Init(this);
        
        // 처음 킬 때 스크롤 제일 위로 올려두기
        _scrollRect = _uiTaskGroup.GetOrAddComponent<ScrollRect>();
    }
    
    private void OnClickBlurBackground()
    {
        ClosePopupUI();
    }

    private void OnClickConfirmButton()
    {
        if (_currentTaskButton == null)
        {
            Logger.LogWarning("Nothing to click");
            return;
        }
        
        // 일과 수행
        PlayerTaskData taskData = _currentTaskButton.PlayerTaskData;
        
        if(Managers.Player.GetStat(Define.PlayerStatType.Fatigue) < taskData.RequirementGold)
        {
            Logger.LogWarning("You do not have enough gold to complete task!");
            return;
        }
        
        Managers.Player.AddStat(Define.PlayerStatType.Fatigue, taskData.FatigueValue);
        Managers.Player.AddStat(Define.PlayerStatType.Experience, taskData.ExperienceValue);
        Managers.Player.AddStat(Define.PlayerStatType.Intelligence, taskData.IntelligenceValue);
        Managers.Player.AddStat(Define.PlayerStatType.GravityAdaptation, taskData.GravityAdaptationValue);
        Managers.Player.AddStat(Define.PlayerStatType.Luck, Random.Range(taskData.LuckMinValue, taskData.LuckMaxValue)); 
        
        // 일과 종료
        ClosePopupUI();
    }
    
    public void SelectTabButton(UITaskTabButton taskTabButton)
    {
        // 현재 고른 Task Type 변경
        if (_currentTaskTab != null)
        {
            _currentTaskTab.Deselect();
        }

        _currentTaskTab = taskTabButton;
        _currentTaskTab.Select();
        SetTaskGroup();
        
        // 스크롤 제일 위로 올리기
        _scrollRect.verticalNormalizedPosition = 1.0f;
    }

    // 타입을 가지고 task 세팅
    private void SetTaskGroup()
    {
        Logger.Log($"{_currentTaskTab.TaskType} task group set");
        if (_uiTaskGroup != null)
        {
            _uiTaskGroup.Setup(_currentTaskTab.TaskType);   
        }

        if (_currentTaskButton != null)
        {
            _currentTaskButton.Deselect();
        }
    }

    public void SelectTaskButton(UITaskButton taskButton)
    {
        // 현재 고른 Task 변경
        if (_currentTaskButton != null)
        {
            _currentTaskButton.Deselect();
        }

        _currentTaskButton = taskButton;
        
        _currentTaskButton.Select();
        SetTaskStatTextImage();
    }

    public void SetTaskStatText()
    {
        PlayerData playerData = Managers.Player.PlayerData;
        GetText((int)Texts.ExperienceValueText).text = $"{playerData.Stats[Define.PlayerStatType.Experience]} / 100";
        GetText((int)Texts.GravityAdaptationValueText).text = $"{playerData.Stats[Define.PlayerStatType.GravityAdaptation]} / 100";
        GetText((int)Texts.IntelligenceValueText).text = $"{playerData.Stats[Define.PlayerStatType.Intelligence]} / 100";
        GetText((int)Texts.LuckValueText).text = $"{playerData.Stats[Define.PlayerStatType.Luck]} / 100";
        SetTaskStatImages(false);
    }

    private void SetTaskStatTextImage()
    {
        PlayerTaskData playerData = _currentTaskButton.PlayerTaskData;
        
        TaskAnimator.TriggerTask(playerData.TaskID);


        SetTaskStatImages(false);
            
        // 능력치 상승치 표기
        if (playerData.ExperienceValue > 0)
        {
            GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        else if (playerData.ExperienceValue < 0)
        {
            GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        
        
        if (playerData.IntelligenceValue > 0)
        {
            GetImage((int)Images.IntelligenceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);   
        }
        else if (playerData.IntelligenceValue < 0)
        {
            GetImage((int)Images.IntelligenceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        
        
        if (playerData.GravityAdaptationValue > 0)
        {
            GetImage((int)Images.GravityAdaptationValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        else if (playerData.GravityAdaptationValue < 0)
        {
            GetImage((int)Images.GravityAdaptationValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        
        if (playerData.LuckMinValue != 0)
        {
            // 운 랜덤 상승
            GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private void SetTaskStatImages(bool active)
    {
        float value = active == true ? 1.0f : 0.0f;
        GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.IntelligenceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.GravityAdaptationValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, value);
        
        GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.IntelligenceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.GravityAdaptationValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        GetImage((int)Images.LuckValueTextDecreaseImage).color = new Color(1f, 1f, 1f, value);
        
    }
}
