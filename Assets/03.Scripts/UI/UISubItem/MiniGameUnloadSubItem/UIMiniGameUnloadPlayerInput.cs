using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIMiniGameUnloadPlayerInput : UIPlayerInput
{

    private Action<int> _skillAction;

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

    public void SetSkillInfo(SkillBase[] skillList)
    {
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

        foreach (var skill in skillList)
        {

            if (skill is CoolingSkill coolingSkill)
            {
                coolingSkill.SetSkillCooldownAction(SetCoolingSkillButtonDuration);
                GetImage((int)Images.CoolingSkillIconImage).sprite = coolingSkill.SkillData.SkillIcon;
            }
            else if (skill is BoxWarpSkill boxWarpSkill)
            {
                boxWarpSkill.SetCountChangedAction(SetBoxWarpSkillCountText);
                GetImage((int)Images.BoxWarpSkillIconImage).sprite = boxWarpSkill.SkillData.SkillIcon;
            }
            else if (skill is SpeedUpSkill speedUpSkill)
            {
                speedUpSkill.SetSkillCooldownAction(SetSpeedUpSkillButtonDuration);
                speedUpSkill.SetActiveAction(SetSpeedUpSkillActiveIcon);
                GetImage((int)Images.SpeedUpSkillIconImage).sprite = speedUpSkill.SkillData.SkillIcon;
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

    public void SetInteractionButtonSprite()
    {
        if (_init)
        {
            int num = Managers.MiniGame.CurrentGame.PlayerController.InteractionActionNumber;
            GetImage((int)Images.InteractionButtonImage).sprite = _spriteList[num];
        }
    }
}
