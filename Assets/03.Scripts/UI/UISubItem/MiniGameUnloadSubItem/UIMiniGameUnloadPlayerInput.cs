using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIMiniGameUnloadPlayerInput : UIPlayerInput
{

    private Action<int> _skillAction;
    private SkillBase[] _skillList;
    private readonly Dictionary<Buttons, Button> _skillButtons = new Dictionary<Buttons, Button>();

    enum Images
    {
        InteractionButtonImage,
        CoolingSkillButtonDurationImage,
        BoxWarpSkillButtonDurationImage,
        SpeedUpSkillButtonDurationImage,
        CoolingSkillIconImage,
        BoxWarpSkillIconImage,
        SpeedUpSkillIconImage,
        UISpeedUpSkillActiveIcon,
    }

    enum Buttons
    {
        CoolingSkillButton,
        BoxWarpSkillButton,
        SpeedUpSkillButton,
    }

    enum Texts
    {
        BoxWarpSkillCountText
    }

    [Header("Interaction Sprites")]
    [SerializeField] private List<Sprite> _spriteList = new List<Sprite>();
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        _init = true;

        BindImage(typeof(Images));
        BindText(typeof(Texts));

        GetImage((int)Images.UISpeedUpSkillActiveIcon).gameObject.SetActive(false);

        return _init;
    }

    private void OnDestroy()
    {
        if (_skillList == null) return;
        
        foreach (var skill in _skillList)
        {
            if (skill is CoolingSkill coolingSkill)
            {
                coolingSkill.OnCooldownChanged -= SetCoolingSkillButtonDuration;
            }
            else if (skill is BoxWarpSkill boxWarpSkill)
            {
                boxWarpSkill.OnCountChanged -= SetBoxWarpSkillCountText;
            }
            else if (skill is SpeedUpSkill speedUpSkill)
            {
                speedUpSkill.OnCooldownChanged -= SetSpeedUpSkillButtonDuration;
                speedUpSkill.OnActiveStateChanged -= SetSpeedUpSkillActiveIcon;
            }
        }
    }

    public void SetSkillInfo(SkillBase[] skillList)
    {
        Init();
        if (_init == false)
        {
            Logger.LogError("UIMiniGameUnloadPlayerInput is not initialized.");
            return;
        }

        if (skillList == null || skillList.Length == 0)
        {
            Logger.LogError("Skill list is empty or null.");
            return;
        }

        _skillList = skillList;

        // 스킬 아이콘, 쿨타임, 지속시간 UI 설정
        foreach (var skill in _skillList)
        {
            if (skill is CoolingSkill coolingSkill)
            {
                coolingSkill.OnCooldownChanged += SetCoolingSkillButtonDuration;
                GetImage((int)Images.CoolingSkillIconImage).sprite = coolingSkill.SkillData.Icon;
                SetSkillButtonInteractable(Buttons.CoolingSkillButton, coolingSkill.SkillData.Type);
            }
            else if (skill is BoxWarpSkill boxWarpSkill)
            {
                boxWarpSkill.OnCountChanged += SetBoxWarpSkillCountText;
                GetImage((int)Images.BoxWarpSkillIconImage).sprite = boxWarpSkill.SkillData.Icon;
                SetSkillButtonInteractable(Buttons.BoxWarpSkillButton, boxWarpSkill.SkillData.Type);
            }
            else if (skill is SpeedUpSkill speedUpSkill)
            {
                speedUpSkill.OnCooldownChanged += SetSpeedUpSkillButtonDuration;
                speedUpSkill.OnActiveStateChanged += SetSpeedUpSkillActiveIcon;
                GetImage((int)Images.SpeedUpSkillIconImage).sprite = speedUpSkill.SkillData.Icon;
                SetSkillButtonInteractable(Buttons.SpeedUpSkillButton, speedUpSkill.SkillData.Type);
            }
        }
    }

    public void SetSkillAction(Action<int> skillAction)
    {
        _skillAction = skillAction;
    }

    public void OnCoolingSkill()
    {
        Logger.Log("Use CoolingSkill");
        _skillAction?.Invoke((int)Buttons.CoolingSkillButton);
    }

    public void OnBoxWarpSkill()
    {
        Logger.Log("Use Box Warp");
        _skillAction?.Invoke((int)Buttons.BoxWarpSkillButton);
    }
    public void OnSpeedUpSkill()
    {
        Logger.Log("Use Speed Up");
        _skillAction?.Invoke((int)Buttons.SpeedUpSkillButton);
    }

    public void SetCoolingSkillButtonDuration(float currentDuration, float maxDuration)
    {
        if (_init)
        {
            GetImage((int)Images.CoolingSkillButtonDurationImage).fillAmount = 1 - currentDuration / maxDuration;
        }
    }

    public void SetBoxWarpSkillCountText(int count)
    {
        if (_init)
        {
            GetText((int)Texts.BoxWarpSkillCountText).text = count.ToString();
            var button = GetSkillButton(Buttons.BoxWarpSkillButton);
            if (button != null)
            {
                bool isInteractable = count > 0;
                button.interactable = isInteractable;
                SetBoxWarpVisualState(button, button.interactable);
            }

            if (count <= 0)
            {
                GetImage((int)Images.BoxWarpSkillButtonDurationImage).fillAmount = 1f;
            }
        }
    }

    public void SetSpeedUpSkillButtonDuration(float currentDuration, float maxDuration)
    {
        if (_init)
        {
            GetImage((int)Images.SpeedUpSkillButtonDurationImage).fillAmount = 1 - currentDuration / maxDuration;
        }
    }

    public void SetSpeedUpSkillActiveIcon(bool isActive)
    {
        if (_init)
        {
            GetImage((int)Images.UISpeedUpSkillActiveIcon).gameObject.SetActive(isActive);
        }
    }

    public void SetInteractionButtonSprite(int num)
    {
        if (_init)
        {
            GetImage((int)Images.InteractionButtonImage).sprite = _spriteList[num];
            Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber = num;
        }
    }

    private void SetSkillButtonInteractable(Buttons buttonType, Define.MiniGameSkillType skillType)
    {
        int skillLevel = 0;
        var playerData = Managers.Player.PlayerData;
        if (playerData != null && playerData.MiniGameUnloadSkillLevel != null)
        {
            if (!playerData.MiniGameUnloadSkillLevel.TryGetValue(skillType, out skillLevel))
            {
                skillLevel = 0;
            }
        }

        var button = GetSkillButton(buttonType);
        if (button != null)
        {
            bool isUnlocked = skillLevel > 0;
            button.gameObject.SetActive(isUnlocked);
            if (!isUnlocked)
            {
                return;
            }

            button.interactable = true;
            if (buttonType == Buttons.BoxWarpSkillButton)
            {
                SetBoxWarpVisualState(button, button.interactable);
            }
        }
    }

    private Button GetSkillButton(Buttons buttonType)
    {
        if (_skillButtons.TryGetValue(buttonType, out var button))
        {
            return button;
        }

        button = Utils.FindChild<Button>(gameObject, buttonType.ToString(), true);
        if (button != null)
        {
            _skillButtons[buttonType] = button;
        }

        return button;
    }

    private void SetBoxWarpVisualState(Button button, bool isInteractable)
    {
        Color targetColor = isInteractable ? button.colors.normalColor : button.colors.disabledColor;
        var icon = GetImage((int)Images.BoxWarpSkillIconImage);
        if (icon != null)
        {
            icon.color = targetColor;
        }

        // var countText = GetText((int)Texts.BoxWarpSkillCountText);
        // if (countText != null)
        // {
        //     countText.color = targetColor;
        // }
    }
}
