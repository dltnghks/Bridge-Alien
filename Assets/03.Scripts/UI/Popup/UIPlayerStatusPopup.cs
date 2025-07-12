using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatusPopup : UIPopup
{
    enum Texts
    {
        ExperienceText,            // 작업 숙련
        GravityAdaptationText,     // 중력 적응
        IntelligenceText,          // 지능
        LuckText,                  // 운
    }
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        
        BindText(typeof(Texts));
        
        SetPlayerStat();

        return true;
    }

    private void SetPlayerStat()
    {
        foreach (Define.PlayerStatsType type in Enum.GetValues(typeof(Define.PlayerStatsType)))
        {
            string value = Managers.Player.GetStat(type).ToString();
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
        GetText((int)Texts.ExperienceText).SetText(newText);
    }
    
    private void SetGravityAdaptationText(string text)
    {
        string newText = text;
        GetText((int)Texts.GravityAdaptationText).SetText(newText);
    }
    private void SetIntelligenceText(string text)
    {
        string newText = text;
        GetText((int)Texts.IntelligenceText).SetText(newText);
    }
    private void SetLuckText(string text)
    {
        string newText = text;
        GetText((int)Texts.LuckText).SetText(newText);
    }

    
}
