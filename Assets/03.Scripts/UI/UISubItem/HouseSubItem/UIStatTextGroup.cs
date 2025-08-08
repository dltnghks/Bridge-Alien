using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStatusTextGroup : UISubItem
{
    enum Texts
    {
        ExperienceValueText,
        GravityValueText,
        IntelligenceValueText,
        LuckValueText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }

        BindText(typeof(Texts));

        return true;
    }

    public void SetPlayerStat()
    {
        Init();
        foreach (Define.PlayerStatsType type in Enum.GetValues(typeof(Define.PlayerStatsType)))
        {
            string value = Managers.Player.GetStats(type).ToString() + "/100";
            switch (type)
            {
                case Define.PlayerStatsType.Experience:
                    SetExperienceText(value);
                    break;
                case Define.PlayerStatsType.GravityAdaptation:
                    SetGravityAdaptationText(value);
                    break;
                case Define.PlayerStatsType.Intelligence:
                    SetIntelligenceText(value);
                    break;
                case Define.PlayerStatsType.Luck:
                    SetLuckText(value);
                    break;
                default:
                    Logger.LogWarning("Player Stats Type Error");
                    break;
            }
        }

    }

    private void SetExperienceText(string text)
    {
        string newText = text;
        GetText((int)Texts.ExperienceValueText).SetText(newText);
    }
    
    private void SetGravityAdaptationText(string text)
    {
        string newText = text;
        GetText((int)Texts.GravityValueText).SetText(newText);
    }
    private void SetIntelligenceText(string text)
    {
        string newText = text;
        GetText((int)Texts.IntelligenceValueText).SetText(newText);
    }
    private void SetLuckText(string text)
    {
        string newText = text;
        GetText((int)Texts.LuckValueText).SetText(newText);
    }
    
}
