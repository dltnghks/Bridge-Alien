using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITaskResultPopup : UIPopup
{
    enum Texts
    {
        ExperienceValueText,
        GravityAdaptationValueText,
        IntelligenceValueText,
        LuckValueText,
        TaskCompletedText,
    }
    
    private PlayerTaskData _playerTaskData;
    
    public override bool Init()
    {
        if (base.Init() == false)
        {
            return false;
        }
        BindText(typeof(Texts));
        
        gameObject.BindEvent(OnClickUI);
        
        return true;
    }

    public override void Init(object data)
    {
        base.Init(data);
        if (data is PlayerTaskData taskData)
        {
            _playerTaskData = taskData;
            
            // 텍스트 세팅
            SetText();
        }
        else
        {
            Logger.LogError("data is not PlayerTaskData");
        }
    }
    
    private void OnClickUI()
    {
        ClosePopupUI();
    }
    
    public void SetText()
    {
        PlayerData playerData = Managers.Player.PlayerData;
        GetText((int)Texts.ExperienceValueText).text = $"{playerData.Stats[Define.PlayerStatType.Experience]} / 100";
        GetText((int)Texts.GravityAdaptationValueText).text = $"{playerData.Stats[Define.PlayerStatType.GravityAdaptation]} / 100";
        GetText((int)Texts.IntelligenceValueText).text = $"{playerData.Stats[Define.PlayerStatType.Intelligence]} / 100";
        GetText((int)Texts.LuckValueText).text = $"{playerData.Stats[Define.PlayerStatType.Luck]} / 100";

        if (_playerTaskData != null)
        {
            GetText((int)Texts.TaskCompletedText).text = _playerTaskData.TaskCompletedText;
        }
    }
    
}
