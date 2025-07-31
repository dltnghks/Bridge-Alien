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
        ThumbnailText,
        ConfirmButtonText,
    }

    enum Images
    {
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
    private UIActiveButton _uiConfirmButton = null;
    private ScrollRect _scrollRect = null;
    
    private PlayerTaskData _selectedTaskData = null;
    
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
        
        _uiConfirmButton = GetButton((int)Buttons.ConfirmButton).gameObject.GetOrAddComponent<UIActiveButton>();
        _uiConfirmButton.Init();
        _uiConfirmButton.gameObject.BindEvent(OnClickConfirmButton);
        
        InitTabGroup();
        InitTaskGroup();
        
        SetTaskStatText();
        
        // 초기화가 끝난 후, 첫 번째 탭 선택
        if (Managers.Player.GetStats(Define.PlayerStatsType.Fatigue) == 0)
        {
            // 피로도가 없으면 휴식&유흥 탭
            SelectTabButton(GetButton((int)Buttons.EntertainmentButton).GetComponent<UITaskTabButton>());    
        }
        else
        {
            // 기본은 자기계발
            SelectTabButton(GetButton((int)Buttons.SelfDevelopmentButton).GetComponent<UITaskTabButton>());
        }
        
        
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

    // 수행하기
    private void OnClickConfirmButton()
    {
        Managers.Sound.PlaySFX(SoundType.CommonSoundSFX, CommonSoundSFX.CommonButtonClick.ToString());
        if (_currentTaskButton == null)
        {
            Logger.LogWarning("Nothing to click");
            return;
        }

        // 피로도, 골드 조건 체크
        if(Managers.Player.GetStats(Define.PlayerStatsType.Fatigue) < _selectedTaskData.RequirementGold)
        {
            Logger.LogWarning("You do not have enough gold to complete task!");
            return;
        }
        
        // 일과 수행창 예약
        Managers.UI.RequestPopup<UITaskProgressPopup>(_selectedTaskData);

        Managers.Player.AddStats(Define.PlayerStatsType.Fatigue, _selectedTaskData.FatigueValue);
        Managers.Player.AddStats(Define.PlayerStatsType.Experience, _selectedTaskData.ExperienceValue);
        Managers.Player.AddStats(Define.PlayerStatsType.Intelligence, _selectedTaskData.IntelligenceValue);
        Managers.Player.AddStats(Define.PlayerStatsType.GravityAdaptation, _selectedTaskData.GravityAdaptationValue);
        Managers.Player.AddStats(Define.PlayerStatsType.Luck, Random.Range(_selectedTaskData.LuckMinValue, _selectedTaskData.LuckMaxValue));
        
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
        
        // 처음 일과 팝업 오픈 시 일과 선택
        bool first = !(_uiTaskGroup.TaskButtons.Count > 1);
        
        if (_uiTaskGroup != null)
        {
            _uiTaskGroup.Setup(_currentTaskTab.TaskType);
            if (first)
            {
                SelectTaskButton(_uiTaskGroup.TaskButtons[0]);
            }
            else
            {
                _currentTaskButton?.Deselect();
            }
        }

        foreach (var taskButton in _uiTaskGroup.TaskButtons)
        {
            if (taskButton.PlayerTaskData == _selectedTaskData)
            {
                SelectTaskButton(taskButton);
                break;
            }
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
            GetText((int)Texts.ConfirmButtonText).text = "소지금 부족";
        }
        else
        {
            _uiConfirmButton?.Activate();
            GetText((int)Texts.ConfirmButtonText).text = "수행하기";
        }
    }

    private void SetTaskStatText()
    {
        PlayerData playerData = Managers.Player.PlayerData;
        GetText((int)Texts.ExperienceValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Experience]} / 100";
        GetText((int)Texts.GravityAdaptationValueText).text = $"{playerData.Stats[Define.PlayerStatsType.GravityAdaptation]} / 100";
        GetText((int)Texts.IntelligenceValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Intelligence]} / 100";
        GetText((int)Texts.LuckValueText).text = $"{playerData.Stats[Define.PlayerStatsType.Luck]} / 100";
        SetTaskStatImages(false);
    }

    private void SetTaskStatTextImage()
    {   
        TaskAnimator.TriggerTask(_selectedTaskData.TaskID);


        SetTaskStatImages(false);
            
        // 능력치 상승치 표기
        if (_selectedTaskData.ExperienceValue > 0)
        {
            GetImage((int)Images.ExperienceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        else if (_selectedTaskData.ExperienceValue < 0)
        {
            GetImage((int)Images.ExperienceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        
        
        if (_selectedTaskData.IntelligenceValue > 0)
        {
            GetImage((int)Images.IntelligenceValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);   
        }
        else if (_selectedTaskData.IntelligenceValue < 0)
        {
            GetImage((int)Images.IntelligenceValueTextDecreaseImage).color = new Color(1f, 1f, 1f, 1f);
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
            // 운 랜덤 상승
            GetImage((int)Images.LuckValueTextIncreaseImage).color = new Color(1f, 1f, 1f, 1f);
        }
        
        // 썸네일 텍스트 설정
        GetText((int)Texts.ThumbnailText).text = _selectedTaskData.ThumbnailText;
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
